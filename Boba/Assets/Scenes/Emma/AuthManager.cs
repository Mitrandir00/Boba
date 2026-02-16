using UnityEngine;
using TMPro;                // Serve per leggere le caselle di testo
using UnityEngine.SceneManagement; // Serve per cambiare scena

public class AuthManager : MonoBehaviour
{
    [Header("UI Riferimenti")]
    public GameObject loginPanel;       // Il pannello nero
    public TMP_InputField usernameInput; // Casella Nome
    public TMP_InputField passwordInput; // Casella Password
    public TextMeshProUGUI messageText;  // Testo Rosso per errori

    [Header("Impostazioni")]
    public string gameSceneName = "Main"; // IL NOME DELLA TUA SCENA DI GIOCO

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
            messageText.text = "Insert name and password!";
            messageText.color = Color.red;
            return;
        }

        // Controllo se esiste già
        if (PlayerPrefs.HasKey("Account_" + user))
        {
            messageText.text = "Username already exists!";
            messageText.color = Color.red;
            return;
        }

        // SALVATAGGIO NUOVO UTENTE
        PlayerPrefs.SetString("Account_" + user, pass);
        PlayerPrefs.Save();

        messageText.text = "Registered successfully! Now click Login.";
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
            messageText.text = "User not found. Register first!";
            messageText.color = Color.red;
            return;
        }

        // 2. La password è giusta?
        string savedPass = PlayerPrefs.GetString("Account_" + user);
        
        if (savedPass == pass)
        {
            // PASSWORD GIUSTA!
            messageText.text = "Welcome " + user + "...";
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
            messageText.text = "Wrong Password!";
            messageText.color = Color.red;
        }
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}