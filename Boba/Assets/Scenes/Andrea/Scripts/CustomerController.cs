using UnityEngine;
using UnityEngine.Events;

public class CustomerController : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] public Transform waitPoint;   // dove attende
    [SerializeField] public Transform exitPoint;   // uscita

    [Header("Movimento")]
    [SerializeField] public float moveSpeed = 2.5f;

    [Header("Attesa")]
    [SerializeField] private float maxWaitSeconds = 10f;

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

    void Awake()
    {
        if (!order)   order   = GetComponentInChildren<CustomerOrder>(true);
        if (!orderUI) orderUI = GetComponentInChildren<CustomerOrderUI>(true);
    }

    void Start()
    {
        state = State.Entering;
        target = waitPoint ? waitPoint.position : transform.position;

        if (orderUI) orderUI.Hide();
    }

    void Update()
    {
        if (state == State.Entering || state == State.Exiting)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                if (state == State.Entering)
                {
                    state = State.Waiting;
                    waitTimer = 0f;
                    OnReadyToOrder?.Invoke(); // -> CustomerOrder.PickAndShowOrder()
                }
                else if (state == State.Exiting)
                {
                    Destroy(gameObject);
                }
            }
        }

        if (state == State.Waiting)
        {
            waitTimer += Time.deltaTime;

            // ⏱️ timeout naturale
            if (waitTimer >= maxWaitSeconds)
            {
                OnWaitTimedOut?.Invoke();
                if (orderUI) orderUI.Stufo();
                BeginExit();
                return;
            }

            // 🔧 MODALITÀ TEST: 1 = corretto, 2 = sbagliato
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (orderUI) orderUI.ShowYesNo(true);
                OnOrderCorrect?.Invoke();
                BeginExit();
                return;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (orderUI) orderUI.ShowYesNo(false);
                OnOrderWrong?.Invoke();
                BeginExit();
                return;
            }
        }
    }

    /// <summary>
    /// Consegna vera (quando avrai il drink costruito dal player).
    /// </summary>
    public void ReceiveDrink(BobaRecipe delivered)
    {
        if (state != State.Waiting || order == null) return;

        bool correct = order.Matches(delivered);

        if (orderUI) orderUI.ShowYesNo(correct);
        if (correct) OnOrderCorrect?.Invoke();
        else         OnOrderWrong?.Invoke();

        BeginExit();
    }

    public void ServeAndExit()
    {
        if (state != State.Waiting) return;
        BeginExit();
    }

    private void BeginExit()
    {
        // lasciamo visibile "SI/NO" durante l'uscita
        state = State.Exiting;
        target = exitPoint ? exitPoint.position : transform.position;
    }
}
