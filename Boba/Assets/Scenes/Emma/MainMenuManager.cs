using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Pannelli")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    public GameObject storyMenuPanel; 
    public GameObject loginPanel;

    [Header("Input")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI feedbackText;

    [Header("Bottoni INTERNI al Pannello (NON SPARISCONO)")]
    public GameObject loginButton;     
    public GameObject registerButton; 

    [Header("Bottoni HOMEPAGE (QUESTI SPARISCONO)")]
    public GameObject pulsanteApriLogin;
       

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Servizi Unity inizializzati con successo!");
            
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                AuthenticationService.Instance.SignOut(); 
            }
        }
        catch (System.Exception e)
        {
            ShowFeedback("Errore di connessione ai servizi: " + e.Message);
        }

        if (PlayerPrefs.GetInt("OpenLevelSelect", 0) == 1)
        {
            PlayerPrefs.SetInt("OpenLevelSelect", 0);
            if(mainMenuPanel) mainMenuPanel.SetActive(false);
            if(storyMenuPanel) storyMenuPanel.SetActive(true);
        }
        else
        {
            ShowMainMenu();
        }
        CheckLoginStatus();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandleBackButton();
        
        if (Input.GetKeyDown(KeyCode.R)) { PlayerPrefs.DeleteAll(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    }

    //CONTROLLO VISIBILITÀ 
    public void CheckLoginStatus()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            string user = PlayerPrefs.GetString("CurrentUser", "Giocatore");
            ShowFeedback("Welcome " + user + "!");
            if (pulsanteApriLogin != null) pulsanteApriLogin.SetActive(false);
        }
        else
        {
            ShowFeedback("Please log in to play.");
            if (pulsanteApriLogin != null) pulsanteApriLogin.SetActive(true);
        }
    }

    // LOGICA DI ACCESSO
    public async void OnLoginClick()
    {
        string u = usernameInput.text;
        string p = passwordInput.text;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) 
        { 
            ShowFeedback("Please enter Username and Password!"); 
            return; 
        }

        ShowFeedback("Logging in...");

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(u, p);
            
            PlayerPrefs.SetString("CurrentUser", u);
            PlayerPrefs.Save();
            
            ShowFeedback("Welcome " + u + "!");
            
            if(storyMenuPanel != null) storyMenuPanel.SetActive(false);
            if(mainMenuPanel != null) mainMenuPanel.SetActive(true);

            CheckLoginStatus(); 
        }
        catch (AuthenticationException ex)
        {
            HandleLoginError(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            HandleLoginError(ex.Message);
        }
    }

    public async void OnRegisterClick()
    {
        string u = usernameInput.text;
        string p = passwordInput.text;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) 
        { 
            ShowFeedback("Please enter Username and Password!"); 
            return; 
        }

        ShowFeedback("Creating account...");

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(u, p);
            ShowFeedback("Registered successfully! Now click Log In.");
            AuthenticationService.Instance.SignOut();
        }
        catch (AuthenticationException ex)
        {
            HandleRegistrationError(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            HandleRegistrationError(ex.Message);
        }
    }

    private void HandleRegistrationError(string errorMsg)
    {
        string msg = errorMsg.ToLower();

        if (msg.Contains("username does not match") || msg.Contains("invalid_username"))
        {
            ShowFeedback("Invalid username! Use 3-20 chars (letters, numbers, and . - _ @ only). No spaces!");
        }
        else if (msg.Contains("password does not match") || msg.Contains("invalid_password") || msg.Contains("weak"))
        {
            ShowFeedback("Weak password! Min. 8 chars, 1 uppercase, 1 lowercase, 1 number, and 1 symbol (e.g., ! @ #).");
        }
        else if (msg.Contains("already exists") || msg.Contains("conflict"))
        {
            ShowFeedback("Username already taken! Please choose another.");
        }
        else
        {
            ShowFeedback("Connection error. Check your internet!");
            Debug.LogError("Server Error: " + errorMsg);
        }
    }

    private void HandleLoginError(string errorMsg)
    {
        string msg = errorMsg.ToLower();

        if (msg.Contains("invalid") || msg.Contains("wrong") || msg.Contains("match") || msg.Contains("unauthorized") || msg.Contains("not found"))
        {
            ShowFeedback("Invalid Username or Password!");
        }
        else
        {
            ShowFeedback("Connection error. Please try again!");
            Debug.LogError("Server Error: " + errorMsg);
        }
    }

    //NAVIGAZIONE
    void ShowFeedback(string msg) { Debug.Log(msg); if (feedbackText != null) feedbackText.text = msg; }
    public void ShowMainMenu() { mainMenuPanel.SetActive(true); if(optionsPanel) optionsPanel.SetActive(false); if(storyMenuPanel) storyMenuPanel.SetActive(false); }
    public void BackToMainMenu() { ShowMainMenu(); }
    private void HandleBackButton() { if (optionsPanel != null && optionsPanel.activeSelf) BackToMainMenu(); else QuitGame(); }
    public void QuitGame() 
    { 
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

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
    }
    public void CloseLoginPanel()
    {
        loginPanel.SetActive(false);
    }
}