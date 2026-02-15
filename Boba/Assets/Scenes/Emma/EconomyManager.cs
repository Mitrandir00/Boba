using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class IngredientPrice
{
    public Ingredient ingredient;
    public int costPerUnit;
}

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance;

    [Header("Listino Prezzi")]
    public List<IngredientPrice> priceList = new List<IngredientPrice>();
    public int defaultCost = 1;

    [Header("Impostazioni Partita")]
    public int startingCoins = 50;

    [Header("Riferimenti UI")]
    public TextMeshProUGUI coinText;
    public GameObject gameOverPanel;

    private int currentCoins;

    // 🔹 PROPERTY PUBBLICA per leggere le monete attuali
    public int CurrentCoins => currentCoins;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentCoins = startingCoins;
        UpdateUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // 🔹 Restituisce il costo di un ingrediente
    public int GetIngredientCost(Ingredient ing)
    {
        foreach (var item in priceList)
        {
            if (item.ingredient == ing)
                return item.costPerUnit;
        }

        Debug.LogWarning("Prezzo mancante nel listino per: " + ing);
        return defaultCost;
    }

    // 🔹 SPENDERE soldi
    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return true;

        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            UpdateUI();

            if (currentCoins == 0)
                Debug.Log("Attenzione: Hai finito i soldi!");

            return true;
        }
        else
        {
            Debug.Log("Non hai abbastanza soldi!");
            TriggerGameOver();
            return false;
        }
    }

    // 🔹 GUADAGNARE soldi
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        currentCoins += amount;
        UpdateUI();
    }

    private void TriggerGameOver()
    {
        Debug.Log("GAME OVER - SEI AL VERDE!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = currentCoins.ToString();
    }
}
