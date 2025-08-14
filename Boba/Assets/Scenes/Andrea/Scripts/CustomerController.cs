using UnityEngine;

public class CustomerController : MonoBehaviour
{
    // Riferimenti ai punti (trascinali dall'Hierarchy dopo)
    public Transform waitPoint;   // dove il cliente si ferma (centro)
    public Transform exitPoint;   // da dove esce (porta a sinistra)

    // Parametri
    public float moveSpeed = 2.5f;

    // Stato interno
    private enum State { Entering, Waiting, Exiting }
    private State state;
    private Vector3 target;

    void Start()
    {
        // appena parte, vai verso il punto di attesa
        state = State.Entering;
        target = waitPoint.position;
    }

    void Update()
    {
        // muovi se stai entrando o uscendo
        if (state == State.Entering || state == State.Exiting)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            // arrivato alla destinazione attuale?
            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                if (state == State.Entering)
                {
                    state = State.Waiting; // fermo al centro, pronto per l'ordine
                }
                else if (state == State.Exiting)
                {
                    Destroy(gameObject);   // cliente uscito
                }
            }
        }

        // TEST: quando è in attesa, premi SPACE per farlo uscire
        if (state == State.Waiting && Input.GetKeyDown(KeyCode.Space))
        {
            state = State.Exiting;
            target = exitPoint.position;
        }
    }
}
