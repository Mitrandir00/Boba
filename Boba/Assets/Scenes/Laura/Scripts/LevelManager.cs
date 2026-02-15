using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Configurazione Livelli")]
    public List<LevelData> levels;

    [Header("Spawner References (metti qui SINISTRA [0] e DESTRA [1])")]
    public List<CustomerSpawner> spawners;

    private LevelData currentLevelData;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (spawners == null || spawners.Count == 0)
        {
            Debug.LogError("[LevelManager] Nessuno spawner assegnato!");
            return;
        }

        if (GameSettings.IsStoryMode)
        {
            LoadStoryLevel(GameSettings.SelectedLevel);
        }
        else
        {
            StartInfiniteMode();
        }
    }

    private void StartInfiniteMode()
    {
        Debug.Log("Modalità Infinita Attivata");

        foreach (var spawner in spawners)
        {
            if (spawner == null) continue;

            // riattiva eventuali spawner spenti dalla story
            spawner.gameObject.SetActive(true);
            spawner.SetLogicEnabled(true);

            spawner.StartInfiniteMode();
        }
    }

    private void LoadStoryLevel(int levelNumber)
    {
        int listIndex = levelNumber - 1;

        if (listIndex < 0 || listIndex >= levels.Count)
        {
            Debug.LogError($"[LevelManager] Livello storia non valido: {levelNumber}");
            return;
        }

        currentLevelData = levels[listIndex];
        Debug.Log("Caricato Livello Storia: " + currentLevelData.name);

        // SOLO SINISTRA = spawners[0]
        CustomerSpawner leftSpawner = spawners[0];

        if (leftSpawner != null)
        {
            leftSpawner.gameObject.SetActive(true);
            leftSpawner.SetLogicEnabled(true);
            leftSpawner.StartStorySequence(currentLevelData.customerSequence, levelNumber);
        }
        else
        {
            Debug.LogError("[LevelManager] Spawner sinistro (index 0) è nullo!");
        }

        // spegni tutti gli altri (destra compresa)
        for (int i = 1; i < spawners.Count; i++)
        {
            var spawner = spawners[i];
            if (spawner == null) continue;

            spawner.StopSpawning();
            spawner.SetLogicEnabled(false);

            // Spegnimento totale (così sei sicuro al 100% che non spawni)
            spawner.gameObject.SetActive(false);
        }
    }

    public bool IsCurrentLevelStory()
    {
        return GameSettings.IsStoryMode;
    }
}
