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
using System.Collections;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Punti di Posizionamento")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform waitPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Prefab Modalità Infinita")]
    [SerializeField] private List<GameObject> randomCustomers = new List<GameObject>();

    [Header("Sfalsamento tra spawner (secondi)")]
    [SerializeField] private float startOffset = 0f; // es. sinistra 0, destra 0.8

    // =========================
    // STORIA: delay tra clienti per livello (per QUESTO spawner)
    // =========================
    [Header("STORIA - Delay spawn per livello (secondi)")]
    [SerializeField] private float storyDelayLevel1 = 3f;
    [SerializeField] private float storyDelayLevel2 = 2.25f;
    [SerializeField] private float storyDelayLevel3 = 1.75f;

    // =========================
    // STORIA: slider (0..1)
    // =========================
    [Header("STORIA - Slider per livello (0..1)")]
    [Range(0f, 1f)] [SerializeField] private float storySpeedTLevel1 = 0f;
    [Range(0f, 1f)] [SerializeField] private float storySpeedTLevel2 = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float storySpeedTLevel3 = 1f;

    [Range(0f, 1f)] [SerializeField] private float storyWaitTLevel1 = 0f;
    [Range(0f, 1f)] [SerializeField] private float storyWaitTLevel2 = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float storyWaitTLevel3 = 1f;

    // =========================
    // INFINITO: scaling
    // =========================
    [Header("INFINITO - Scaling difficoltà (spawn)")]
    [SerializeField] private float infiniteStartDelay = 4f;
    [SerializeField] private float infiniteMinDelay = 0.4f;
    [SerializeField] private float secondsToReachMinDelay = 60f;

    [Header("INFINITO - Scaling tuning cliente (0..1)")]
    [SerializeField] private float infiniteSpeedMultiplier = 1f;
    [SerializeField] private float infiniteWaitMultiplier = 1f;

    [Header("Generale")]
    [SerializeField] private float safetyDelayAfterSpawn = 0.1f;

    // =========================
    // Stato interno
    // =========================
    private List<GameObject> storySequence;
    private int storyIndex;

    private GameObject currentCustomer;
    private Coroutine routine;

    // Se disabilitiamo solo la logica ma lasciamo l'oggetto attivo (comodo se non vuoi SetActive false)
    private bool logicEnabled = true;

    // =========================
    // API
    // =========================

    /// <summary>Abilita/Disabilita la logica di spawn (non distrugge il cliente in scena).</summary>
    public void SetLogicEnabled(bool enabled)
    {
        logicEnabled = enabled;
        if (!logicEnabled)
            StopSpawning();
    }

    /// <summary>Ferma la routine attuale (storia o infinito). Non distrugge il cliente già presente.</summary>
    public void StopSpawning()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    public void StartStorySequence(List<GameObject> sequence, int storyLevelIndex)
    {
        StopSpawning();

        storySequence = sequence;
        storyIndex = 0;

        if (!logicEnabled || !gameObject.activeInHierarchy)
            return;

        routine = StartCoroutine(StoryRoutine(storyLevelIndex));
    }

    public void StartInfiniteMode()
    {
        StopSpawning();

        if (!logicEnabled || !gameObject.activeInHierarchy)
            return;

        routine = StartCoroutine(InfiniteRoutine());
    }

    private void OnDisable()
    {
        // se lo spegni in Inspector o da codice, non lascia coroutine appese
        StopSpawning();
    }

    // =========================
    // ROUTINE STORIA
    // =========================
    private IEnumerator StoryRoutine(int storyLevelIndex)
    {
        if (startOffset > 0f)
            yield return new WaitForSeconds(startOffset);

        float delayBetweenCustomers = GetStoryDelay(storyLevelIndex);
        float speedT = GetStorySpeedT(storyLevelIndex);
        float waitT = GetStoryWaitT(storyLevelIndex);

        while (logicEnabled && storySequence != null && storyIndex < storySequence.Count)
        {
            // 1 cliente per spawner alla volta
            yield return new WaitUntil(() => !logicEnabled || currentCustomer == null);
            if (!logicEnabled) yield break;

            SpawnCustomer(storySequence[storyIndex], speedT, waitT);

            if (safetyDelayAfterSpawn > 0f)
                yield return new WaitForSeconds(safetyDelayAfterSpawn);

            // aspetta che il cliente del MIO spawner se ne vada
            yield return new WaitUntil(() => !logicEnabled || currentCustomer == null);
            if (!logicEnabled) yield break;

            if (delayBetweenCustomers > 0f)
                yield return new WaitForSeconds(delayBetweenCustomers);

            storyIndex++;
        }

        routine = null;
    }

    private float GetStoryDelay(int lvl)
    {
        switch (lvl)
        {
            case 1: return storyDelayLevel1;
            case 2: return storyDelayLevel2;
            case 3: return storyDelayLevel3;
            default: return storyDelayLevel1;
        }
    }

    private float GetStorySpeedT(int lvl)
    {
        switch (lvl)
        {
            case 1: return storySpeedTLevel1;
            case 2: return storySpeedTLevel2;
            case 3: return storySpeedTLevel3;
            default: return storySpeedTLevel1;
        }
    }

    private float GetStoryWaitT(int lvl)
    {
        switch (lvl)
        {
            case 1: return storyWaitTLevel1;
            case 2: return storyWaitTLevel2;
            case 3: return storyWaitTLevel3;
            default: return storyWaitTLevel1;
        }
    }

    // =========================
    // ROUTINE INFINITO
    // =========================
    private IEnumerator InfiniteRoutine()
    {
        if (startOffset > 0f)
            yield return new WaitForSeconds(startOffset);

        float startTime = Time.time;

        while (logicEnabled)
        {
            float elapsed = Time.time - startTime;
            float t = (secondsToReachMinDelay <= 0f) ? 1f : Mathf.Clamp01(elapsed / secondsToReachMinDelay);

            // Delay spawn scende col tempo
            float currentDelay = Mathf.Lerp(infiniteStartDelay, infiniteMinDelay, t);

            if (currentCustomer == null)
            {
                if (randomCustomers != null && randomCustomers.Count > 0)
                {
                    GameObject prefab = randomCustomers[Random.Range(0, randomCustomers.Count)];

                    float speedT = Mathf.Clamp01(t * infiniteSpeedMultiplier);
                    float waitT = Mathf.Clamp01(t * infiniteWaitMultiplier);

                    SpawnCustomer(prefab, speedT, waitT);
                }
                else
                {
                    Debug.LogWarning($"[{name}] randomCustomers è vuota!");
                }
            }

            if (!logicEnabled) yield break;

            if (currentDelay > 0f)
                yield return new WaitForSeconds(currentDelay);
            else
                yield return null;
        }

        routine = null;
    }

    // =========================
    // SPAWN
    // =========================
    private void SpawnCustomer(GameObject prefab, float speedT, float waitT)
    {
        if (!logicEnabled) return;

        if (prefab == null)
        {
            Debug.LogWarning($"[{name}] Prefab nullo, impossibile spawnare.");
            return;
        }
        if (spawnPoint == null)
        {
            Debug.LogError($"[{name}] spawnPoint non assegnato!");
            return;
        }

        currentCustomer = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        var controller = currentCustomer.GetComponent<CustomerController>();
        if (controller != null)
        {
            controller.waitPoint = waitPoint;
            controller.exitPoint = exitPoint;

            // tuning (range definiti nel CustomerController)
            controller.ApplyTuning(speedT, waitT);
        }
        else
        {
            Debug.LogWarning($"[{name}] Il prefab {prefab.name} non ha CustomerController!");
        }
    }
}
