using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Pannelli UI")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject storyMenuPanel;

    [Header("Bottoni")]
    public GameObject loginButton;

    void Start()
    {
        // Assicura che all'avvio sia visibile solo il menu principale
        ShowMainMenu();

        // --- CONTROLLO LOGIN ---
        if (PlayerPrefs.HasKey("CurrentUser"))
        {
            // Utente loggato: nascondi il tasto login
            if (loginButton != null) loginButton.SetActive(false);
            Debug.Log("Utente rilevato: " + PlayerPrefs.GetString("CurrentUser") + ". Nascondo il login.");
        }
        else
        {
            // Utente non loggato: mostra il tasto login
            if (loginButton != null) loginButton.SetActive(true);
            Debug.Log("Nessun utente loggato. Mostro il login.");
        }
    }

    void Update()
    {
        // Gestione tasto Back Android / ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }

    // --- LOGICA DI NAVIGAZIONE ---

    public void ShowMainMenu()
    {
        if(mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if(optionsPanel != null) optionsPanel.SetActive(false);
        if(storyMenuPanel != null) storyMenuPanel.SetActive(false);
    }

    private void HandleBackButton()
    {
        if (optionsPanel.activeSelf || storyMenuPanel.activeSelf)
        {
            BackToMainMenu();
        }
        else if (mainMenuPanel.activeSelf)
        {
            QuitGame();
        }
    }

    // --- PULSANTI ---

    public void OpenStoryMenu()
    {
        mainMenuPanel.SetActive(false);
        storyMenuPanel.SetActive(true);
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    public void QuitGame()
    {
        Debug.Log("Chiusura gioco...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // --- AVVIO GIOCO ---

    public void StartInfiniteMode()
    {
        // Assicurati che la classe GameSettings esista nel tuo progetto
        GameSettings.IsStoryMode = false;
        GameSettings.SelectedLevel = 0;
        
        Debug.Log("Avvio Modalità Infinita");
        SceneManager.LoadScene("Main");
    }

    public void StartStoryLevel(int levelIndex)
    {
        GameSettings.IsStoryMode = true;
        GameSettings.SelectedLevel = levelIndex;
        
        Debug.Log("Avvio Livello Storia: " + levelIndex);
        SceneManager.LoadScene("Main");
    }
}