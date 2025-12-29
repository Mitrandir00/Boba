using UnityEngine;
using TMPro; // Serve per il testo delle monete
using System.Collections.Generic; // Fondamentale per usare le Liste
using UnityEngine.SceneManagement; // Serve per ricaricare la scena se perdi

// Questa classe serve solo per creare la coppia "Ingrediente - Prezzo" nell'Inspector
[System.Serializable]
public class IngredientPrice
{
    public Ingredient ingredient; // Il tipo di ingrediente
    public int costPerUnit;       // Quanto costa una singola unità
}

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance; 

    [Header("Listino Prezzi")]
    // Qui definisci quanto costa ogni cosa. Riempi questa lista dall'Inspector
    public List<IngredientPrice> priceList = new List<IngredientPrice>();
    public int defaultCost = 1; // Costo di sicurezza se ti dimentichi di inserire un prezzo

    [Header("Impostazioni Partita")]
    public int startingCoins = 50;

    [Header("Riferimenti UI")]
    public TextMeshProUGUI coinText; // Trascina qui il testo dell'UI
    public GameObject gameOverPanel; // Il pannello "Hai Perso!"

    private int currentCoins;

    private void Awake()
    {
        // Impostazione Singleton sicura
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 1. Inizializza i soldi
        currentCoins = startingCoins;
        UpdateUI();
        
        // Assicurati che il Game Over sia spento all'inizio
        if(gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    //  Cerca il prezzo nel listino
    // Viene chiamata dall'OptionSpawner per sapere quanto far pagare il drink
    public int GetIngredientCost(Ingredient ing)
    {
        // Cerca nella lista l'ingrediente richiesto
        foreach (var item in priceList)
        {
            if (item.ingredient == ing)
            {
                return item.costPerUnit;
            }
        }
        // Se non lo trovi, usa il costo di default
        Debug.LogWarning("Prezzo mancante nel listino per: " + ing);
        return defaultCost; 
    }
    

    // Funzione per SPENDERE soldi
    // Restituisce "true" se l'acquisto è riuscito, "false" se non hai abbastanza soldi
    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateUI();
            
            // Controllo extra: se vai esattamente a 0, potremmo voler avvisare
            if (currentCoins == 0) Debug.Log("Attenzione: Hai finito i soldi!");
            
            return true; // Acquisto riuscito
        }
        else
        {
            Debug.Log("Non hai abbastanza soldi per questo acquisto!");
            TriggerGameOver(); // Se provi a spendere senza soldi, scatta il Game Over
            return false; // Acquisto fallito
        }
    }

    // Funzione per GUADAGNARE soldi (quando vendi)
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI();
    }

    void TriggerGameOver()
    {
        Debug.Log("GAME OVER - SEI AL VERDE!");
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // Ferma il gioco completamente
        }
    }

    void UpdateUI()
    {
        if (coinText != null)
            coinText.text = "Monete: " + currentCoins.ToString();
    }
}