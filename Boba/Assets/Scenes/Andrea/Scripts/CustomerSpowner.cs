using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Prefabs dei clienti (trascina qui i 5)")]
    public GameObject[] customerPrefabs;  // 5 prefab diversi

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
}
