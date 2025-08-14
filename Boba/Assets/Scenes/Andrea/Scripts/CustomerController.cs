using UnityEngine;
using UnityEngine.Events;

public class CustomerController : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform waitPoint;      // centro
    public Transform exitPoint;      // porta (sinistra)

    [Header("Movement")]
    public float moveSpeed = 2.5f;

    [Header("Waiting")]
    public float maxWaitSeconds = 10f;    // ⏳ tempo massimo d'attesa
    public UnityEvent OnReadyToOrder;     // 🔔 scatta quando arriva al centro
    public UnityEvent OnWaitTimedOut;     // (opzionale) scatta quando scade il tempo

    private enum State { Entering, Waiting, Exiting }
    private State state;
    private Vector3 target;
    private float waitTimer;              // timer interno in secondi

    void Start()
    {
        state = State.Entering;
        target = waitPoint.position;
    }

    void Update()
    {
        // Movimento verso il target (entrata/uscita)
        if (state == State.Entering || state == State.Exiting)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                if (state == State.Entering)
                {
                    // Arrivato al centro: inizia l'attesa
                    state = State.Waiting;
                    waitTimer = 0f;              // reset timer
                    OnReadyToOrder?.Invoke();    // segnala che è pronto a ordinare
                }
                else if (state == State.Exiting)
                {
                    Destroy(gameObject);         // despawn dopo l'uscita
                }
            }
        }

        // Gestione attesa al centro
        if (state == State.Waiting)
        {
            // ⏳ accumula il tempo d'attesa
            waitTimer += Time.deltaTime;

            // Se arriva a 10s (o al valore impostato) → va via da solo
            if (waitTimer >= maxWaitSeconds)
            {
                OnWaitTimedOut?.Invoke();  // opzionale, per punteggio/feedback
                BeginExit();
            }

            // (solo per test manuale) SPACE = servi prima del timeout
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ServeAndExit();
            }
        }
    }

    // 👉 chiamalo quando la bibita viene consegnata (prima del timeout)
    public void ServeAndExit()
    {
        if (state != State.Waiting) return;
        BeginExit();
    }

    private void BeginExit()
    {
        state = State.Exiting;
        target = exitPoint.position;
    }
}
