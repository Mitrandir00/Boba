using UnityEngine;
using UnityEngine.Events;

public class CustomerController : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform waitPoint;      // punto centrale dove il cliente attende
    public Transform exitPoint;      // uscita a sinistra

    [Header("Movimento")]
    public float moveSpeed = 2.5f;

    [Header("Attesa")]
    public float maxWaitSeconds = 10f;    // tempo massimo di attesa
    public UnityEvent OnReadyToOrder;     // invocato quando raggiunge il waitPoint
    public UnityEvent OnWaitTimedOut;     // invocato se scade l'attesa

    [Header("UI")]
    [SerializeField] private CustomerOrderUI orderUI;  // per nascondere/mostrare il balloon

    private enum State { Entering, Waiting, Exiting }
    private State state;
    private Vector3 target;
    private float waitTimer;

    void Start()
    {
        state = State.Entering;
        target = waitPoint.position;

        // Per sicurezza: balloon nascosto fin dall’inizio.
        if (orderUI) orderUI.Hide();
    }

    void Update()
    {
        // Movimento (entrata/uscita)
        if (state == State.Entering || state == State.Exiting)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, target, moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                if (state == State.Entering)
                {
                    // Arrivato al centro: passa in attesa e notifica che è pronto a ordinare
                    state = State.Waiting;
                    waitTimer = 0f;
                    OnReadyToOrder?.Invoke(); // CustomerOrder.PickAndShowOrder → fa apparire il balloon
                }
                else if (state == State.Exiting)
                {
                    Destroy(gameObject);
                }
            }
        }

        // Gestione fase di attesa al centro
        if (state == State.Waiting)
        {
            waitTimer += Time.deltaTime;

            // Timeout: va via e nascondiamo il balloon
            if (waitTimer >= maxWaitSeconds)
            {
                OnWaitTimedOut?.Invoke();
                BeginExit();
            }

            // (Opzionale: test manuale) premi SPAZIO per servirlo e farlo uscire
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ServeAndExit();
            }
        }
    }

    /// <summary>
    /// Chiamare quando il cliente viene servito prima del timeout.
    /// </summary>
    public void ServeAndExit()
    {
        if (state != State.Waiting) return;
    
        BeginExit();
    }

    private void BeginExit()
    {
        // Nascondi balloon quando inizia a uscire
        if (orderUI) orderUI.Hide();
        state = State.Exiting;
        target = exitPoint.position;
    }
}
