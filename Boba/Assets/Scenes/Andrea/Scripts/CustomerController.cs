using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CustomerController : MonoBehaviour
{
    // =========================
    // VISUAL / ESPRESSIONI
    // =========================
    [Header("Visual Feedback (Varianti Espressioni)")]
    [SerializeField] private SpriteRenderer faceSR;

    [SerializeField] private Sprite[] neutralSprites;
    [SerializeField] private Sprite[] happySprites;
    [SerializeField] private Sprite[] sadSprites;

    // =========================
    // WAYPOINTS (assegnati dallo spawner)
    // =========================
    [Header("Waypoints")]
    public Transform waitPoint;
    public Transform exitPoint;

    [Header("Ritardo Pensiero")]
    public float thinkingTime = 0.1f; // Il cliente aspetta 0.5 secondi prima di parlare
    // =========================
    // RANGE MODIFICABILI SOLO DA CODICE
    // =========================
    private const float MIN_MOVE_SPEED = 2.5f;
    private const float MAX_MOVE_SPEED = 12.0f;

    private const float MIN_WAIT_SECONDS = 2.0f;
    private const float MAX_WAIT_SECONDS = 14.0f;

    // =========================
    // VALORI RUNTIME (calcolati dagli slider)
    // =========================
    [Header("Runtime (read-only)")]
    [SerializeField] private float moveSpeed = MIN_MOVE_SPEED;
    [SerializeField] private float maxWaitSeconds = MAX_WAIT_SECONDS;

    [Header("Movimento")]
    [SerializeField] private float arriveThreshold = 0.01f;

    // =========================
    // SALTELLO DI ASSESTAMENTO
    // =========================
    [Header("Saltello di assestamento (al waitPoint)")]
    [SerializeField] private bool enableSettleHop = true;
    [SerializeField] private float hopHeight = 0.30f;
    [SerializeField] private float hopUpTime = 0.05f;
    [SerializeField] private float hopDownTime = 0.010f;

    // =========================
    // EVENTI
    // =========================
    [Header("Eventi di stato")]
    public UnityEvent OnReadyToOrder;
    public UnityEvent OnWaitTimedOut;

    [Header("Reazioni alla consegna (opzionali)")]
    public UnityEvent OnOrderCorrect;
    public UnityEvent OnOrderWrong;

    // =========================
    // ORDINE & UI
    // =========================
    [Header("Ordine & UI")]
    [SerializeField] private CustomerOrder order;
    [SerializeField] private CustomerOrderUI orderUI;

    // =========================
    // ECONOMIA
    // =========================
    [Header("Economia")]
    public int rewardAmount = 20;

    // =========================
    // STATO
    // =========================
    private enum State { Entering, Waiting, Exiting }
    private State state;

    private Vector3 target;
    private float waitTimer;
    private bool settling;

    // =========================
    // CLIENTI SPECIALI
    // =========================
    private SpoiledCatNoPay spoiledCat; // se presente -> niente soldi

    // =========================
    // UNITY LIFECYCLE
    // =========================
    private void Awake()
    {
        if (!order) order = GetComponentInChildren<CustomerOrder>(true);
        if (!orderUI) orderUI = GetComponentInChildren<CustomerOrderUI>(true);

        // cache cliente speciale (se sul prefab c'è)
        spoiledCat = GetComponent<SpoiledCatNoPay>();

        // valori di default
        moveSpeed = MIN_MOVE_SPEED;
        maxWaitSeconds = MAX_WAIT_SECONDS;
    }

    private void Start()
    {
        if (orderUI) orderUI.Hide();
        SetRandomSprite(neutralSprites);

        state = State.Entering;
        target = waitPoint ? waitPoint.position : transform.position;
    }

    private void Update()
    {
        HandleMovement();
        HandleWaiting();
    }

    // =========================
    // API PER LO SPAWNER (SLIDER 0..1)
    // =========================
    public void ApplyTuning(float speedT, float waitT)
    {
        speedT = Mathf.Clamp01(speedT);
        waitT = Mathf.Clamp01(waitT);

        moveSpeed = Mathf.Lerp(MIN_MOVE_SPEED, MAX_MOVE_SPEED, speedT);
        maxWaitSeconds = Mathf.Lerp(MAX_WAIT_SECONDS, MIN_WAIT_SECONDS, waitT);
    }

    // =========================
    // MOVIMENTO
    // =========================
    private void HandleMovement()
    {
        if ((state != State.Entering && state != State.Exiting) || settling) return;

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) > arriveThreshold) return;

        transform.position = target;

        if (state == State.Entering)
        {
            if (enableSettleHop) StartCoroutine(ArriveAndSettleRoutine());
            else EnterWaitingState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ArriveAndSettleRoutine()
    {
        settling = true;
        yield return StartCoroutine(SettleHop());
        settling = false;
        EnterWaitingState();
    }

    // =========================
    // ATTESA
    // =========================
    private void EnterWaitingState()
    {
        state = State.Waiting;
        waitTimer = 0f;
       StartCoroutine(ThinkingRoutine());
    }

    private IEnumerator ThinkingRoutine()
    {
        // Aspetta i secondi che hai deciso
        yield return new WaitForSeconds(thinkingTime);

        // ORA fa apparire il balloon con il testo
        OnReadyToOrder?.Invoke();
    }

    private void HandleWaiting()
    {
        if (state != State.Waiting) return;
        
        // SE è modalità storia, saltiamo il timer di uscita
        if (GameSettings.IsStoryMode) return;

        waitTimer += Time.deltaTime;

        if (waitTimer < maxWaitSeconds) return;

        OnWaitTimedOut?.Invoke();
        if (orderUI) orderUI.Stufo();
        if (order) order.ClearUI();

        BeginExit();
    }

    // =========================
    // USCITA
    // =========================
    private void BeginExit()
    {
        state = State.Exiting;
        target = exitPoint ? exitPoint.position : transform.position;
    }

    // =========================
    // CONSEGNA
    // =========================
    public void ReceiveDrink(BobaRecipe delivered)
    {
        if (state != State.Waiting || order == null) return;

        bool correct = order.Matches(delivered);
        order.ClearUI();

        if (orderUI) orderUI.ShowYesNo(correct);

        if (correct)
        {
            SetRandomSprite(happySprites);

            // ✅ Se NON è un gatto viziato, paga
            if (spoiledCat == null)
            {
                if (EconomyManager.instance != null)
                    EconomyManager.instance.AddCoins(rewardAmount);
            }
            // se è viziato: non dà soldi, ma è comunque "corretto"
            OnOrderCorrect?.Invoke();
        }
        else
        {
            SetRandomSprite(sadSprites);
            OnOrderWrong?.Invoke();
        }

        BeginExit();
    }

    // =========================
    // SALTELLO
    // =========================
    private IEnumerator SettleHop()
    {
        Vector3 basePos = transform.position;
        Vector3 upPos = basePos + Vector3.up * hopHeight;

        float t = 0f;
        while (t < hopUpTime)
        {
            t += Time.deltaTime;
            float x = Mathf.Clamp01(t / hopUpTime);
            float e = 1f - Mathf.Pow(1f - x, 2f);
            transform.position = Vector3.LerpUnclamped(basePos, upPos, e);
            yield return null;
        }
        transform.position = upPos;

        t = 0f;
        while (t < hopDownTime)
        {
            t += Time.deltaTime;
            float x = Mathf.Clamp01(t / hopDownTime);
            float e = x * x;
            transform.position = Vector3.LerpUnclamped(upPos, basePos, e);
            yield return null;
        }
        transform.position = basePos;
    }

    // =========================
    // UTILS
    // =========================
    private void SetRandomSprite(Sprite[] spriteArray)
    {
        if (faceSR == null || spriteArray == null || spriteArray.Length == 0) return;
        faceSR.sprite = spriteArray[Random.Range(0, spriteArray.Length)];
    }
}
