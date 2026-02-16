using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Serve per cambiare scena
using System.Collections;

public class MafiaCatController : MonoBehaviour
{
    [Header("Impostazioni Furto")]
    [Range(0f, 1f)] public float stealPercent = 0.50f; 
    public string mafiaDialogue = "Finito! Dammi i soldi...";

    [Header("Riferimenti")]
    public Transform waitPoint;
    public Transform exitPoint;
    public float moveSpeed = 3.5f;

    [Header("UI & Balloon")]
    public CustomerOrderUI orderUI;
    public Button balloonButton;

    private void Start()
    {
        if (!orderUI) orderUI = GetComponentInChildren<CustomerOrderUI>();
        
        // Disattiva il bottone all'inizio
        if (balloonButton) balloonButton.interactable = false;

        StartCoroutine(RobRoutine());
    }

    private IEnumerator RobRoutine()
    {
        // 1. Vai al centro
        yield return MoveTo(waitPoint.position);

        // 2. Ruba i soldi (con DEBUG per capire il problema)
        StealMoney();

        // 3. Mostra il Balloon
        if (orderUI)
        {
            orderUI.ShowTextOrder(mafiaDialogue);
            
            if (balloonButton) 
            {
                balloonButton.interactable = true;
                // Rimuove vecchi click e aggiunge il nuovo
                balloonButton.onClick.RemoveAllListeners();
                balloonButton.onClick.AddListener(OnBalloonClicked);
            }
        }
    }

    // QUESTA FUNZIONE PARTE QUANDO CLICCHI IL BALLOON
    public void OnBalloonClicked()
    {
        Debug.Log("Gatto cliccato: Fine Livello!");

        // 1. Spegni UI
        if (orderUI) orderUI.Hide();

        // 2. Salva un "messaggio" per il Menu Principale
        // "1" significa: Appena il menu si apre, vai alla selezione livelli
        PlayerPrefs.SetInt("OpenLevelSelect", 1);
        PlayerPrefs.Save();

        // 3. Carica la scena del Menu (Assicurati si chiami "MainMenu")
        SceneManager.LoadScene("MainMenu");
    }

    private void StealMoney()
    {
        // Controllo 1: Esiste il manager?
        if (EconomyManager.instance == null)
        {
            Debug.LogError("ERRORE: EconomyManager non trovato nella scena! Il gatto non può rubare.");
            return;
        }

        // Controllo 2: Hai soldi?
        int currentCoins = EconomyManager.instance.CurrentCoins;
        if (currentCoins <= 0)
        {
            Debug.LogWarning("Il gatto voleva rubare, ma hai 0 monete! (Quindi ruba 0)");
            return;
        }

        // Esegui il furto
        int stolen = Mathf.FloorToInt(currentCoins * stealPercent);
        EconomyManager.instance.SpendCoins(stolen);
        
        Debug.Log($"FURTO RIUSCITO: Avevi {currentCoins}, rubati {stolen}.");
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}