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
    public GameObject loginButton;     // Il tasto "Conferma Login" dentro il pannello
    public GameObject registerButton;  // Il tasto "Conferma Registrazione" dentro il pannello

    [Header("Bottoni HOMEPAGE (QUESTI SPARISCONO)")]
    public GameObject pulsanteApriLogin; //Il bottone nella Home che APRE il menu login
       // Il bottone Gioca

    void Start()
    {
        ShowMainMenu();
        CheckLoginStatus();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandleBackButton();
        
        // Tasto Reset per i test (Tasto R)
        if (Input.GetKeyDown(KeyCode.R)) { PlayerPrefs.DeleteAll(); SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    }

    // --- CONTROLLO VISIBILITÀ ---
    public void CheckLoginStatus()
    {
        if (PlayerPrefs.HasKey("CurrentUser"))
        {
            // === UTENTE LOGGATO ===
            string user = PlayerPrefs.GetString("CurrentUser");
            ShowFeedback("Welcome " + user + "!");

            // 1. NASCONDI IL BOTTONE CHE APRE IL MENU (Quello nella Home)
            if (pulsanteApriLogin != null) pulsanteApriLogin.SetActive(false);


            // NOTA: I tasti loginButton e registerButton NON vengono toccati, restano attivi!
        }
        else
        {
            // === NESSUNO LOGGATO ===
            ShowFeedback("Effettua l'accesso per giocare.");

            // 1. MOSTRA IL BOTTONE CHE APRE IL MENU
            if (pulsanteApriLogin != null) pulsanteApriLogin.SetActive(true);

        }
    }

    // --- LOGICA DI ACCESSO ---
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

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) { ShowFeedback("Missing fields!"); return; }
        
        if (PlayerPrefs.HasKey(u)) { ShowFeedback("Username already taken!"); }
        else 
        { 
            PlayerPrefs.SetString(u, p); 
            ShowFeedback("Registered! Now click Login to log in."); 
        }
    }

    // --- NAVIGAZIONE ---
    void ShowFeedback(string msg) { Debug.Log(msg); if (feedbackText != null) feedbackText.text = msg; }
    public void ShowMainMenu() { mainMenuPanel.SetActive(true); if(optionsPanel) optionsPanel.SetActive(false); if(storyMenuPanel) storyMenuPanel.SetActive(false); }
    public void BackToMainMenu() { ShowMainMenu(); }
    private void HandleBackButton() { if (optionsPanel != null && optionsPanel.activeSelf) BackToMainMenu(); else QuitGame(); }
    public void QuitGame() { Application.Quit(); }
    
    // Funzioni per aprire i pannelli
    public void OpenOptions() { if(mainMenuPanel) mainMenuPanel.SetActive(false); if(optionsPanel) optionsPanel.SetActive(true); }
    
    // QUESTA È QUELLA CHE APRE IL LOGIN
    public void OpenStoryMenu() { if(mainMenuPanel) mainMenuPanel.SetActive(false); if(storyMenuPanel) storyMenuPanel.SetActive(true); }
    
    public void StartStoryLevel(int i) { SceneManager.LoadScene("Main"); }
    public void StartInfiniteMode() { SceneManager.LoadScene("Main"); }
}