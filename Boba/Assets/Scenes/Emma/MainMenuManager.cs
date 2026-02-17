using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Pannelli")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject storyMenuPanel; // Pannello Login/Registrazione

    [Header("Input")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    [Header("Bottoni INTERNI al Pannello (NON SPARISCONO)")]
    public GameObject loginButton;     
    public GameObject registerButton; 

    [Header("Bottoni HOMEPAGE (QUESTI SPARISCONO)")]
    public GameObject pulsanteApriLogin;
       

    void Start()
    {
        // 1. Controlla se dobbiamo aprire direttamente la selezione livelli
        if (PlayerPrefs.GetInt("OpenLevelSelect", 0) == 1)
        {
            // Resetta il messaggio (così non lo fa sempre)
            PlayerPrefs.SetInt("OpenLevelSelect", 0);
            
            // Apri direttamente il menu storia
            if(mainMenuPanel) mainMenuPanel.SetActive(false);
            if(storyMenuPanel) storyMenuPanel.SetActive(true);
        }
        else
        {
            // Comportamento normale (Home Page)
            ShowMainMenu();
        }

        CheckLoginStatus();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandleBackButton();
        
        // Tasto Reset per i test (Tasto R)
        if (Input.GetKeyDown(KeyCode.R)) { PlayerPrefs.DeleteAll(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    }

    //CONTROLLO VISIBILITÀ 
    public void CheckLoginStatus()
    {
        if (PlayerPrefs.HasKey("CurrentUser"))
        {
            //UTENTE LOGGATO 
            string user = PlayerPrefs.GetString("CurrentUser");
            ShowFeedback("Welcome back " + user + "!");

            //NASCONDI IL BOTTONE CHE APRE IL MENU (Quello nella Home)
            if (pulsanteApriLogin != null) pulsanteApriLogin.SetActive(false);

        }
        else
        {
            //NESSUNO LOGGATO
            ShowFeedback("Log in to play.");

            // MOSTRA IL BOTTONE CHE APRE IL MENU
            if (pulsanteApriLogin != null) pulsanteApriLogin.SetActive(true);

        }
    }

    // LOGICA DI ACCESSO
    public void OnLoginClick()
    {
        string u = usernameInput.text;
        string p = passwordInput.text;

        if (!PlayerPrefs.HasKey(u)) { ShowFeedback("User not found!"); return; }

        if (PlayerPrefs.GetString(u) == p)
        {
            PlayerPrefs.SetString("CurrentUser", u);
            
            // Chiudiamo il pannello del login perché abbiamo finito
            if(storyMenuPanel != null) storyMenuPanel.SetActive(false);
            if(mainMenuPanel != null) mainMenuPanel.SetActive(true);

            // Aggiorniamo la Home (farà sparire il pulsante di apertura)
            CheckLoginStatus(); 
        }
        else { ShowFeedback("Wrong Password!"); }
    }

    public void OnRegisterClick()
    {
        string u = usernameInput.text;
        string p = passwordInput.text;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) { ShowFeedback("Missing credentials!"); return; }
        
        if (PlayerPrefs.HasKey(u)) { ShowFeedback("User already exists!"); }
        else 
        { 
            PlayerPrefs.SetString(u, p); 
            ShowFeedback("Registered! Now click Log In."); 
        }
    }

    //NAVIGAZIONE
    void ShowFeedback(string msg) { Debug.Log(msg); if (feedbackText != null) feedbackText.text = msg; }
    public void ShowMainMenu() { mainMenuPanel.SetActive(true); if(optionsPanel) optionsPanel.SetActive(false); if(storyMenuPanel) storyMenuPanel.SetActive(false); }
    public void BackToMainMenu() { ShowMainMenu(); }
    private void HandleBackButton() { if (optionsPanel != null && optionsPanel.activeSelf) BackToMainMenu(); else QuitGame(); }
    public void QuitGame() 
    { 
        //Aggiungi un Log per essere sicuro che il bottone sia collegato
        Debug.Log("Sto chiudendo il gioco...");

        #if UNITY_EDITOR
            // Se siamo nell'Editor, ferma la modalità Play
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Se siamo nel gioco vero (Build), chiude l'applicazione
            Application.Quit();
        #endif
    }
    
    // Funzioni per aprire i pannelli
    public void OpenOptions() { if(mainMenuPanel) mainMenuPanel.SetActive(false); if(optionsPanel) optionsPanel.SetActive(true); }
    
    public void OpenStoryMenu() { if(mainMenuPanel) mainMenuPanel.SetActive(false); if(storyMenuPanel) storyMenuPanel.SetActive(true); }
    
    public void StartStoryLevel(int levelIndex) 
    { 
        // 1. Impostiamo la modalità storia
        GameSettings.IsStoryMode = true;
        
        // 2. Salviamo quale livello vogliamo (1, 2 o 3)
        GameSettings.SelectedLevel = levelIndex;

        // 3. Carichiamo la scena di gioco 
        SceneManager.LoadScene("Main"); 
    }
    public void StartInfiniteMode() 
    { 
        // 1. Disattiviamo la modalità storia
        GameSettings.IsStoryMode = false;
        
        // 2. Reset del livello
        GameSettings.SelectedLevel = 0;

        // 3. Carica la scena
        SceneManager.LoadScene("Main"); 
    }
}