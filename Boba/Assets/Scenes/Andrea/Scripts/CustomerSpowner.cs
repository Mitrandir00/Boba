/*using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Prefabs dei clienti (trascina qui i 5)")]
    public GameObject[] customerPrefabs;  // 5 prefab diversi

    [Header("Spawn Area")]
    // Trascina l'oggetto con il BoxCollider2D "SpawnWall" qui nell'Inspector
    public BoxCollider2D spawnAreaCollider;

    [Header("Waypoints")]
    public Transform spawnPoint;  // a sinistra, fuori scena (es. -11,0,0)
    public Transform waitPoint;   // centro (0,0,0)
    public Transform exitPoint;   // porta a sinistra (es. -9,0,0)

    [Header("Timing")]
    public float spawnInterval = 5f;   // ogni quanti secondi provare a spawnare
    public bool spawnOnStart = true;   // spawna subito il primo

    private GameObject current;        // cliente attuale in scena (1 alla volta)

    void Start()
    {
        if (spawnOnStart) SpawnRandom();
        StartCoroutine(RespawnLoop());
    }

    IEnumerator RespawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (current == null)       // spawna solo se non c'è già un cliente
            {
                SpawnRandom();
            }
        }
    }

    void SpawnRandom()
    {
        if (customerPrefabs == null || customerPrefabs.Length == 0) return;

        int idx = Random.Range(0, customerPrefabs.Length); // indice casuale 0..4
        current = Instantiate(customerPrefabs[idx], spawnPoint.position, Quaternion.identity);

        // collega i waypoint allo script del cliente
        var ctrl = current.GetComponent<CustomerController>();
        ctrl.waitPoint = waitPoint;
        ctrl.exitPoint = exitPoint;
    }
    private Vector2 GetRandomSpawnPosition()
    {
        if (spawnAreaCollider == null)
        {
            Debug.LogError("Il BoxCollider2D di spawnAreaCollider non è stato assegnato!");
            return Vector2.zero; // Ritorna (0,0) in caso di errore
        }

        // Ottieni i limiti del BoxCollider2D (World Space)
        Bounds bounds = spawnAreaCollider.bounds;

        // Calcola una posizione X casuale tra il bordo sinistro (min.x) e il bordo destro (max.x)
        float randomX = Random.Range(bounds.min.x, bounds.max.x);

        // Calcola una posizione Y casuale tra il bordo inferiore (min.y) e il bordo superiore (max.y)
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        // Crea un nuovo Vector2 con le coordinate casuali
        Vector2 randomPosition = new Vector2(randomX, randomY);

        return randomPosition;
    }
    private void SpawnCustomer()
    {
        // Ottieni la posizione casuale all'interno del collider
        Vector2 spawnPos = GetRandomSpawnPosition(); 
        
        // Scegli un Prefab a caso dai tuoi Prefabs dei clienti
        int randomIndex = Random.Range(0, customerPrefabs.Length); 
        GameObject customerPrefab = customerPrefabs[randomIndex];

        // Istanzia il cliente alla posizione calcolata
        Instantiate(customerPrefab, spawnPos, Quaternion.identity); 
        
        // ... (Il resto della logica di spawn e Waypoints)
    }
}
*/
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Punti di Posizionamento")]
    public Transform spawnPoint;
    public Transform waitPoint;
    public Transform exitPoint;

    [Header("Impostazioni Modalità Infinita")]
    public List<GameObject> randomCustomers; // Metti qui i tuoi prefab per la modalità infinita
    public float infiniteSpawnDelay = 3f;

    [Header("Impostazioni Generali")]
    public float delayBetweenCustomers = 2f;

    private List<GameObject> storySequence;
    private int currentStoryIndex = 0;

    void Start()
    {
        // Se siamo in modalità infinita, facciamo partire il loop infinito
        if (!GameSettings.IsStoryMode)
        {
            StartCoroutine(InfiniteSpawnRoutine());
        }
    }

    // --- LOGICA MODALITÀ STORIA ---
    public void StartStorySequence(List<GameObject> sequence)
    {
        storySequence = sequence;
        currentStoryIndex = 0;
        StartCoroutine(SpawnSequenceRoutine());
    }

    private IEnumerator SpawnSequenceRoutine()
    {
        while (currentStoryIndex < storySequence.Count)
        {
            SpawnCustomer(storySequence[currentStoryIndex]);
            
            // Aspetta finché non ci sono più oggetti con il tag "Customer" nella scena
            yield return new WaitForSeconds(1f); // Piccolo delay per sicurezza
            yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Customer") == null);
            
            yield return new WaitForSeconds(delayBetweenCustomers);
            currentStoryIndex++;
        }
        Debug.Log("Livello Storia Terminato!");
    }

    // --- LOGICA MODALITÀ INFINITA ---
    /*private IEnumerator InfiniteSpawnRoutine()
    {
        while (true) // Ciclo infinito
        {
            if (GameObject.FindGameObjectWithTag("Customer") == null)
            {
                GameObject randomPrefab = randomCustomers[Random.Range(0, randomCustomers.Count)];
                SpawnCustomer(randomPrefab);
            }
            yield return new WaitForSeconds(infiniteSpawnDelay);
        }
    }*/

    private void SpawnCustomer(GameObject prefab)
    {
        GameObject newCustomer = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        CustomerController controller = newCustomer.GetComponent<CustomerController>();
        
        if (controller != null)
        {
            controller.waitPoint = waitPoint;
            controller.exitPoint = exitPoint;
        }

        // IMPORTANTE: il prefab DEVE avere il tag "Customer"
        newCustomer.tag = "Customer"; 
    }
    // Aggiungi questo metodo pubblico nello script CustomerSpawner
    public void StartInfiniteMode()
    {
        StopAllCoroutines(); // Sicurezza per non sovrapporre i cicli
        StartCoroutine(InfiniteSpawnRoutine());
    }

    private IEnumerator InfiniteSpawnRoutine()
    {
        // Ciclo infinito: finché giochi, lui prova a spawnare
        while (true) 
        {
            // Controlla se c'è già un cliente in scena (usando il Tag "Customer")
            if (GameObject.FindGameObjectWithTag("Customer") == null)
            {
                if (randomCustomers.Count > 0)
                {
                    // Prende un cliente a caso dalla lista che hai nell'Inspector
                    GameObject randomPrefab = randomCustomers[Random.Range(0, randomCustomers.Count)];
                    SpawnCustomer(randomPrefab);
                }
                else
                {
                    Debug.LogWarning("La lista Random Customers dello spawner è vuota!");
                }
            }
            
            // Aspetta qualche secondo prima di controllare di nuovo
            yield return new WaitForSeconds(infiniteSpawnDelay);
        }
    }
}