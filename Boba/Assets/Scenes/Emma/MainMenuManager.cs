using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
<<<<<<< Updated upstream
    
=======
    public GameObject storyMenuPanel;

    public GameObject loginButton;

>>>>>>> Stashed changes
    void Start()
    {
        // Mostra menu principale all'avvio
        ShowMainMenu();

        // Controlliamo se l'utente è già loggato
        if (PlayerPrefs.HasKey("CurrentUser"))
        {
            // --- UTENTE GIÀ DENTRO ---
            // Nascondiamo solo il login, il resto rimane uguale
            if(loginButton != null) 
                loginButton.SetActive(false);
            
            Debug.Log("Utente rilevato. Nascondo il login.");
        }
        else
        {
            // --- NESSUNO LOGGATO ---
            // Assicuriamoci che il login sia visibile per chi vuole entrare
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
        optionsPanel.SetActive(false);
    }
    
    // === PULSANTI MENU PRINCIPALE ===
    
    // === PULSANTI MENU PRINCIPALE ===

    public void PlayGame() 
    {
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
        //non ho fatto solo l application quit perché aggiungendo queste righe posso controllare
        //che funzioni correttamente il bottone fermando la simulazione nell editor di unity (funziona)
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
    
    