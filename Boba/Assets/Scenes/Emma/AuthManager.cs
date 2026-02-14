using UnityEngine;
using TMPro;                
using UnityEngine.SceneManagement; 

public class AuthManager : MonoBehaviour
{
    [Header("UI Riferimenti")]
    public GameObject loginPanel;       
    public TMP_InputField usernameInput; 
    public TMP_InputField passwordInput; 
    public TextMeshProUGUI messageText;  

    [Header("Impostazioni")]
    public string gameSceneName = "MainMenu"; // IL NOME DELLA TUA SCENA DI GIOCO

    void Start()
    {
        // All'inizio chiudiamo il pannello e puliamo il testo
        if (loginPanel != null) loginPanel.SetActive(false);
        if (messageText != null) messageText.text = "";
    }

    // --- TASTO 1: APRIRE IL LOGIN ---
    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        messageText.text = ""; 
    }

    // --- TASTO 2: CHIUDERE IL LOGIN ---
    public void CloseLoginPanel()
    {
        loginPanel.SetActive(false);
    }

    // --- TASTO 3: REGISTRATI ---
    public void RegisterUser()
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        // Controllo se i campi sono vuoti
        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            messageText.text = "Inserisci nome e password!";
            messageText.color = Color.red;
            return;
        }

        // Controllo se esiste già
        if (PlayerPrefs.HasKey("Account_" + user))
        {
            messageText.text = "Nome utente già esistente!";
            messageText.color = Color.red;
            return;
        }

        // SALVATAGGIO NUOVO UTENTE
        PlayerPrefs.SetString("Account_" + user, pass);
        PlayerPrefs.Save();

        messageText.text = "Registrato con successo! Ora clicca Login.";
        messageText.color = Color.green;
    }

    // --- TASTO 4: LOGIN ---
    public void LoginUser()
    {
        string user = usernameInput.text;
        string pass = passwordInput.text;

        // 1. L'utente esiste?
        if (!PlayerPrefs.HasKey("Account_" + user))
        {
            messageText.text = "Utente non trovato. Registrati prima!";
            messageText.color = Color.red;
            return;
        }

        // 2. La password è giusta?
        string savedPass = PlayerPrefs.GetString("Account_" + user);
        
        if (savedPass == pass)
        {
            // PASSWORD GIUSTA!
            messageText.text = "Benvenuto " + user + "...";
            messageText.color = Color.green;

            // Salviamo chi è l'utente attuale (per i soldi)
            PlayerPrefs.SetString("CurrentUser", user);
            PlayerPrefs.Save();

            // Carichiamo il gioco dopo 1 secondo
            Invoke("LoadGameScene", 1f); 
        }
        else
        {
            // PASSWORD SBAGLIATA
            messageText.text = "Password Errata!";
            messageText.color = Color.red;
        }
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}