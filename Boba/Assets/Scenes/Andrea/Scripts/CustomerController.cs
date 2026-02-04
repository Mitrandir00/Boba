using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CustomerController : MonoBehaviour
{
    //per le espressioni del cliente
    [Header("Visual Feedback (Varianti Espressioni)")]
    [SerializeField] private SpriteRenderer faceSR;

    [Tooltip("Inserisci 3 sprite neutri")]
    [SerializeField] private Sprite[] neutralSprites;

    [Tooltip("Inserisci 2 sprite felici")]
    [SerializeField] private Sprite[] happySprites;

    [Tooltip("Inserisci 2 sprite scontenti")]
    [SerializeField] private Sprite[] sadSprites;

    
    [Header("Waypoints")]
    [SerializeField] public Transform waitPoint;   // dove attende
    [SerializeField] public Transform exitPoint;   // uscita

    [Header("Movimento")]
    [SerializeField] public float moveSpeed = 2.5f;

    [Header("Attesa")]
    [SerializeField] private float maxWaitSeconds = 10f;

    [Header("Economia")] // NUOVO
    public int rewardAmount = 20; // Quanto paga questo cliente se soddisfatto?

    [Header("Saltello di assestamento (al waitPoint)")]
    [SerializeField] private bool enableSettleHop = true;
    [SerializeField] private float hopHeight = 0.30f;    // piccolo: 0.04 - 0.12
    [SerializeField] private float hopUpTime = 0.05f;    // veloce
    [SerializeField] private float hopDownTime = 0.010f; // leggermente più lento

    [Header("Eventi di stato")]
    public UnityEvent OnReadyToOrder;   // collega -> CustomerOrder.PickAndShowOrder()
    public UnityEvent OnWaitTimedOut;

    [Header("Ordine & UI")]
    [SerializeField] private CustomerOrder order;      // stesso CustomerOrder del cliente
    [SerializeField] private CustomerOrderUI orderUI;  // balloon dello stesso cliente

    [Header("Reazioni alla consegna (opzionali)")]
    public UnityEvent OnOrderCorrect;   // hook per SFX/VFX/punteggio++
    public UnityEvent OnOrderWrong;     // hook per SFX/VFX/punteggio--

    private enum State { Entering, Waiting, Exiting }
    private State state;
    private Vector3 target;
    private float waitTimer;

    // evita di far partire più volte la coroutine mentre Update gira
    private bool settling = false;


    [Header("Impostazioni Livello")]
    public bool isStoryMode = false; // Se true, il cliente non scappa mai

    void Awake()
    {
        if (!order) order = GetComponentInChildren<CustomerOrder>(true);
        if (!orderUI) orderUI = GetComponentInChildren<CustomerOrderUI>(true);
    }

    /*void Start()
    {
        state = State.Entering;
        target = waitPoint ? waitPoint.position : transform.position;

        if (orderUI) orderUI.Hide();
    }*/
    void Start()
    {
        // SINCRONIZZAZIONE AUTOMATICA:
        // Invece di impostarlo a mano, il cliente legge se è in modalità storia
        isStoryMode = GameSettings.IsStoryMode;

        state = State.Entering;
        target = waitPoint ? waitPoint.position : transform.position;

        if (orderUI) orderUI.Hide();
        // Sceglie un'espressione neutra casuale all'arrivo
        SetRandomSprite(neutralSprites);
    }

    void Update()
    {
        // movimento in entrata/uscita
        if ((state == State.Entering || state == State.Exiting) && !settling)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                if (state == State.Entering)
                {
                    // snap preciso (così il saltello parte davvero "alla fine")
                    transform.position = target;

                    if (enableSettleHop)
                    {
                        // blocca Update e fai saltello, poi entra in Waiting
                        StartCoroutine(ArrivedAtWaitPointRoutine());
                    }
                    else
                    {
                        // nessun saltello
                        EnterWaitingState();
                    }
                }
                else if (state == State.Exiting)
                {
                    Destroy(gameObject);
                }
            }
        }

        // logica di attesa
        if (state == State.Waiting)
        {
            // SE è modalità storia, non incrementiamo il timer e non usciamo per tempo
            if (isStoryMode) return;

            waitTimer += Time.deltaTime;
            // ⏱️ timeout naturale
            if (waitTimer >= maxWaitSeconds)
            {
                OnWaitTimedOut?.Invoke();
                if (orderUI) orderUI.Stufo();

                // opzionale: pulisci UI ordine se vuoi (di solito sì)
                if (order) order.ClearUI();

                BeginExit();
                return;
            }
        }
    }

    /// <summary>
    /// Routine chiamata appena il cliente ARRIVA al waitPoint:
    /// fa il saltello di assestamento e poi attiva ordine/UI.
    /// </summary>
    private IEnumerator ArrivedAtWaitPointRoutine()
    {
        settling = true;

        yield return StartCoroutine(SettleHop());

        settling = false;

        EnterWaitingState();
    }

    private void EnterWaitingState()
    {
        state = State.Waiting;
        waitTimer = 0f;

        // Ora che è fermo e "assestato", mostri l’ordine
        OnReadyToOrder?.Invoke(); // -> CustomerOrder.PickAndShowOrder()
    }

    private IEnumerator SettleHop()
    {
        Vector3 basePos = transform.position;
        Vector3 upPos = basePos + Vector3.up * hopHeight;

        // su (ease-out)
        float t = 0f;
        while (t < hopUpTime)
        {
            t += Time.deltaTime;
            float x = Mathf.Clamp01(t / hopUpTime);
            float e = 1f - Mathf.Pow(1f - x, 2f); // easeOutQuad
            transform.position = Vector3.LerpUnclamped(basePos, upPos, e);
            yield return null;
        }
        transform.position = upPos;

        // giù (ease-in)
        t = 0f;
        while (t < hopDownTime)
        {
            t += Time.deltaTime;
            float x = Mathf.Clamp01(t / hopDownTime);
            float e = x * x; // easeInQuad
            transform.position = Vector3.LerpUnclamped(upPos, basePos, e);
            yield return null;
        }

        transform.position = basePos;
    }

    /// <summary>
    /// Consegna vera (quando avrai il drink costruito dal player).
    /// </summary>
    public void ReceiveDrink(BobaRecipe delivered)
    {
        if (state != State.Waiting || order == null) return;

        bool correct = order.Matches(delivered);
        order.ClearUI();

        if (orderUI) orderUI.ShowYesNo(correct);

        if (correct)
        {
            // Reazione Felice Casuale
            SetRandomSprite(happySprites);

            if (EconomyManager.instance != null)
            {
                EconomyManager.instance.AddCoins(rewardAmount);
            }
            OnOrderCorrect?.Invoke();
        }
        else
        {
            // Reazione Scontenta Casuale
            SetRandomSprite(sadSprites);
            OnOrderWrong?.Invoke();
        }

        BeginExit();
    }

    public void ServeAndExit()
    {
        if (state != State.Waiting) return;

        // opzionale: se lo servi “a prescindere”, nascondi ordine comunque
        if (order) order.ClearUI();

        BeginExit();
    }

    private void BeginExit()
    {
        // lasciamo visibile "SI/NO" durante l'uscita
        state = State.Exiting;
        target = exitPoint ? exitPoint.position : transform.position;
    }

    public void LeaveSatisfied()
    {
        // nascondi ordine prima del feedback
        if (order) order.ClearUI();

        if (orderUI) orderUI.ShowYesNo(true);
        BeginExit();
    }

    public void LeaveDisappointed()
    {
        // nascondi ordine prima del feedback
        if (order) order.ClearUI();

        if (orderUI) orderUI.ShowYesNo(false);
        BeginExit();
    }
    private void SetRandomSprite(Sprite[] spriteArray)
    {
        if (faceSR == null || spriteArray == null || spriteArray.Length == 0) return;

        int randomIndex = Random.Range(0, spriteArray.Length);
        faceSR.sprite = spriteArray[randomIndex];
    }

}