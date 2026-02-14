using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Pannelli UI")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    
    // Questi sono quelli che avevi aggiunto tu e che vogliamo tenere
    public GameObject storyMenuPanel;
    public GameObject loginButton;

    void Start()
    {
        // Assicura che all'avvio sia visibile solo il menu principale
        ShowMainMenu();

        // --- CODICE LOGIN CHE ABBIAMO FATTO OGGI ---
        // Controlliamo se l'utente è già loggato
        if (PlayerPrefs.HasKey("CurrentUser"))
        {
            // UTENTE GIÀ DENTRO -> Nascondiamo il login
            if(loginButton != null) 
                loginButton.SetActive(false);
            
            Debug.Log("Utente rilevato. Nascondo il login.");
        }
        else
        {
            // NESSUNO LOGGATO -> Mostriamo il login
            if(loginButton != null) 
                loginButton.SetActive(true);
        }
    }

    void Update()
    {
        // Gestione tasto Back Android / ESC per chiudere i menu o il gioco
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }

    // --- LOGICA DI NAVIGAZIONE ---

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        storyMenuPanel.SetActive(false);
    }

    private void HandleBackButton()
    {
        // Se siamo in un sottomenu, torna al principale. Altrimenti, esce.
        if (optionsPanel.activeSelf || storyMenuPanel.activeSelf)
        {
            BackToMainMenu();
        }
        else if (mainMenuPanel.activeSelf)
        {
            QuitGame();
        }
    }
    
    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        if(optionsPanel != null) optionsPanel.SetActive(false);
        if(storyMenuPanel != null) storyMenuPanel.SetActive(false);
    }
    
    // === PULSANTI MENU PRINCIPALE ===

    // --- PULSANTI MENU PRINCIPALE ---

    public void OpenStoryMenu()
    {
        mainMenuPanel.SetActive(false);
        storyMenuPanel.SetActive(true);
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        Debug.Log("Menu opzioni aperto");
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        // Nell'editor: ferma il Play Mode
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Nella build: chiude l'applicazione
        Application.Quit();
        #endif
    }
    
    // === PULSANTI MENU OPZIONI ===
    
    public void BackToMainMenu()
    {
        ShowMainMenu();
        Debug.Log("Tornato al menu principale");
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

    // --- AVVIO MODALITÀ DI GIOCO ---

    public void StartInfiniteMode()
    {
        // Configura le impostazioni globali per la modalità infinita
        GameSettings.IsStoryMode = false;
        GameSettings.SelectedLevel = 0;
        
        Debug.Log("Avvio Modalità Infinita");
        SceneManager.LoadScene("Main");
    }

    public void StartStoryLevel(int levelIndex)
    {
        // Configura le impostazioni globali per il livello specifico della storia
        GameSettings.IsStoryMode = true;
        GameSettings.SelectedLevel = levelIndex;
        
        Debug.Log("Avvio Livello Storia: " + levelIndex);
        SceneManager.LoadScene("Main");
    }
}