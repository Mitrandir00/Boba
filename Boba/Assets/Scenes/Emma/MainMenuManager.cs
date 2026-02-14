using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    
    // Questi sono quelli che avevi aggiunto tu e che vogliamo tenere
    public GameObject storyMenuPanel;
    public GameObject loginButton;

    void Start()
    {
        // Mostra menu principale all'avvio
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
        // Gestione tasto Back Android / ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }
    
    // === NAVIGAZIONE ===
    
    void HandleBackButton()
    {
        if (optionsPanel.activeSelf)
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

    public void PlayGame() 
    {
        // Nota: Assicurati che la scena si chiami "Main"
        SceneManager.LoadScene("Main"); 
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
}