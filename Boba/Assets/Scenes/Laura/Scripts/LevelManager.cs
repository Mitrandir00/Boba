using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Configurazione Livelli")]
    public List<LevelData> levels;

    [Header("Spawner References (metti qui SINISTRA e DESTRA)")]
    public List<CustomerSpawner> spawners;

    private LevelData currentLevelData;

    void Awake()
    {
        instance = this;
    }

    void Start()
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
            Debug.Log("Modalità Infinita Attivata");

            foreach (var spawner in spawners)
            {
                if (spawner != null)
                    spawner.StartInfiniteMode();
            }
        }
    }

    void LoadStoryLevel(int index)
    {
        int listIndex = index - 1;

        if (listIndex < 0 || listIndex >= levels.Count)
        {
            Debug.LogError($"[LevelManager] Livello storia non valido: {index}");
            return;
        }

        currentLevelData = levels[listIndex];
        Debug.Log("Caricato Livello Storia: " + currentLevelData.name);

        foreach (var spawner in spawners)
        {
            if (spawner != null)
                spawner.StartStorySequence(currentLevelData.customerSequence, index);
        }
    }

    public bool IsCurrentLevelStory()
    {
        return GameSettings.IsStoryMode;
    }
}
