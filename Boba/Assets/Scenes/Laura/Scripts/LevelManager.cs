using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Configurazione Livelli")]
    public List<LevelData> levels; // Trascina qui i tuoi ScriptableObjects dei livelli
    
    [Header("Spawner Reference")]
    public CustomerSpawner spawner; // Il componente che crea i clienti

    private LevelData currentLevelData;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (GameSettings.IsStoryMode)
        {
            LoadStoryLevel(GameSettings.SelectedLevel);
        }
        else
        {
            Debug.Log("Modalità Infinita Attivata");
            // Qui istruisci lo spawner a far uscire clienti a caso all'infinito
            // Dice allo spawner di iniziare a far apparire clienti casuali
            spawner.StartInfiniteMode();
        }
    }

    void LoadStoryLevel(int index)
    {
        // Gli indici partono da 1 per la storia (0 è infinita nei tuoi GameSettings)
        int listIndex = index - 1; 

        if (listIndex >= 0 && listIndex < levels.Count)
        {
            currentLevelData = levels[listIndex];
            Debug.Log("Caricato Livello Storia: " + currentLevelData.name);
            
            // Passa la sequenza dei clienti allo spawner
            spawner.StartStorySequence(currentLevelData.customerSequence);
        }
    }

    public bool IsCurrentLevelStory() 
    {
        return GameSettings.IsStoryMode;
    }
}