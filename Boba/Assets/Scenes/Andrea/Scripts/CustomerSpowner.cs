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
    public Transform spawnPoint;
    public Transform waitPoint;
    public Transform exitPoint;

    [Header("Prefab Modalità Infinita")]
    public List<GameObject> randomCustomers;

    [Header("Sfalsamento tra spawner (secondi)")]
    public float startOffset = 0f; // es. sinistra 0, destra 0.8

    // =========================
    // STORIA: ritmo spawn (tempo tra un cliente e il successivo per QUESTO spawner)
    // =========================
    [Header("STORIA - Delay spawn per livello (secondi)")]
    public float storyDelayLevel1 = 3f;
    public float storyDelayLevel2 = 2.25f;
    public float storyDelayLevel3 = 1.75f;

    // =========================
    // STORIA: slider di tuning per livello (0..1)
    // - SpeedT: 0 -> MIN_MOVE_SPEED, 1 -> MAX_MOVE_SPEED (definiti nel CustomerController)
    // - WaitT : 0 -> MAX_WAIT_SECONDS, 1 -> MIN_WAIT_SECONDS (definiti nel CustomerController)
    // =========================
    [Header("STORIA - Slider per livello (0..1)")]
    [Range(0f, 1f)] public float storySpeedTLevel1 = 0f;
    [Range(0f, 1f)] public float storySpeedTLevel2 = 0.5f;
    [Range(0f, 1f)] public float storySpeedTLevel3 = 1f;

    [Range(0f, 1f)] public float storyWaitTLevel1 = 0f;
    [Range(0f, 1f)] public float storyWaitTLevel2 = 0.5f;
    [Range(0f, 1f)] public float storyWaitTLevel3 = 1f;

    // =========================
    // INFINITO: scaling difficoltà
    // - Delay tra "tentativi" di spawn scende nel tempo
    // - t (0..1) cresce nel tempo e viene usato per speed/wait via ApplyTuning(t,t)
    // =========================
    [Header("INFINITO - Scaling difficoltà (spawn)")]
    public float infiniteStartDelay = 4f;
    public float infiniteMinDelay = 0.4f;
    public float secondsToReachMinDelay = 60f;

    [Header("INFINITO - Scaling tuning cliente (0..1)")]
    [Tooltip("Quanto rapidamente cresce la VELOCITÀ del cliente in infinito (moltiplica t). Es: 1 = normale, 1.2 = più aggressivo.")]
    public float infiniteSpeedMultiplier = 1f;

    [Tooltip("Quanto rapidamente cresce la IMPAZIENZA del cliente in infinito (moltiplica t). Es: 1 = normale, 0.8 = meno aggressivo.")]
    public float infiniteWaitMultiplier = 1f;

    // =========================
    // GENERALE
    // =========================
    [Header("Generale")]
    public float safetyDelayAfterSpawn = 0.1f;

    private List<GameObject> storySequence;
    private int currentStoryIndex = 0;
    private GameObject currentCustomer;
    private Coroutine runningRoutine;

    // =========================
    // API chiamate dal LevelManager
    // =========================
    public void StartStorySequence(List<GameObject> sequence, int storyLevelIndex)
    {
        StopRunning();

        storySequence = sequence;
        currentStoryIndex = 0;

        runningRoutine = StartCoroutine(StoryRoutine(storyLevelIndex));
    }

    public void StartInfiniteMode()
    {
        StopRunning();
        runningRoutine = StartCoroutine(InfiniteRoutine());
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

        while (storySequence != null && currentStoryIndex < storySequence.Count)
        {
            // 1 cliente per spawner alla volta
            yield return new WaitUntil(() => currentCustomer == null);

            SpawnCustomer(storySequence[currentStoryIndex], speedT, waitT);

            // micro-sicurezza
            if (safetyDelayAfterSpawn > 0f)
                yield return new WaitForSeconds(safetyDelayAfterSpawn);

            // aspetta che il MIO cliente se ne vada
            yield return new WaitUntil(() => currentCustomer == null);

            // delay tra clienti in story
            if (delayBetweenCustomers > 0f)
                yield return new WaitForSeconds(delayBetweenCustomers);

            currentStoryIndex++;
        }

        Debug.Log($"[{name}] Story terminata.");
        runningRoutine = null;
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

        while (true)
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

            if (currentDelay > 0f)
                yield return new WaitForSeconds(currentDelay);
            else
                yield return null;
        }
    }

    // =========================
    // SPAWN
    // =========================
    private void SpawnCustomer(GameObject prefab, float speedT, float waitT)
    {
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

            // ✅ range in codice + slider in inspector
            controller.ApplyTuning(speedT, waitT);
        }
        else
        {
            Debug.LogWarning($"[{name}] Il prefab {prefab.name} non ha CustomerController!");
        }
    }

    // =========================
    // STOP / RESET
    // =========================
    private void StopRunning()
    {
        if (runningRoutine != null)
        {
            StopCoroutine(runningRoutine);
            runningRoutine = null;
        }

        StopAllCoroutines();

        // Nota: non distruggiamo il cliente già in scena.
        // Semplicemente lo spawner considera "occupato" finché non sparisce.
        // Se vuoi resettare forzato anche il cliente, dimmelo e lo facciamo.
    }
}
