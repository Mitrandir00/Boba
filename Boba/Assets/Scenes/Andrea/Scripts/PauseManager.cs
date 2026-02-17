using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Trascina qui l'oggetto 'PauseMenu' intero")]
    public GameObject pauseMenuPanel;
    
    [Header("Oggetti Audio")]
    public AudioSource backgroundMusic;

    [Header("Logout")] // <--- NUOVO: Aggiungi questo header
    public GameObject logoutButton; // <--- NUOVO: La variabile per il bottone

    private bool isPaused = false;

    void Start()
    {
        // Il menu è chiuso quando parte il gioco
        if(pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // --- CONTROLLO VISIBILITÀ LOGOUT (NUOVO) ---
        // Se siamo nel menu principale e apriamo le opzioni, controlliamo se mostrare il logout
        if (logoutButton != null)
        {
            if (PlayerPrefs.HasKey("CurrentUser"))
            {
                logoutButton.SetActive(true); // Se sei loggato, mostra "Esci"
            }
            else
            {
                logoutButton.SetActive(false); // Se non sei loggato, nascondilo
            }
        }
    }

    //FUNZIONE 1: Apre il menu di pausa
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; 
        isPaused = true;
    }

    //FUNZIONE 2: Riprende il gioco
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
    }

    //FUNZIONE 3: Torna alla Homepage
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }

    //FUNZIONE 4: esce dal gioco
    public void QuitGame() 
    { 
        // 1. Aggiungi un Log per essere sicuro che il bottone sia collegato
        Debug.Log("Sto chiudendo il gioco...");

        #if UNITY_EDITOR
            // Se siamo nell'Editor, ferma la modalità Play
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // Se siamo nel gioco vero (Build), chiude l'applicazione
            Application.Quit();
        #endif
    }

    // FUNZIONE 5: Riavvia il livello corrente
    public void RestartLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // FUNZIONE VOLUME
    public void SetVolume(float volume)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }
    }

    // --- NUOVA FUNZIONE: LOGOUT ---
    public void LogOut()
    {
        // 1. Scongela il gioco (importante se lo fai mentre sei in pausa)
        Time.timeScale = 1f;

        // 2. Cancella i dati dell'utente
        PlayerPrefs.DeleteKey("CurrentUser");
        Debug.Log("Logout effettuato da PauseManager!");

        // 3. Ricarica la scena del Menu Principale per resettare tutto (e mostrare il Login)
        SceneManager.LoadScene("MainMenu");
    }
    void Update()
    {
        // Esempio: Se premo ESC, inverte lo stato
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) 
                ResumeGame();
            else 
                PauseGame();
        }
    }
}