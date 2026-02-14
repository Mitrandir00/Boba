using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Trascina qui l'oggetto 'PauseMenu' intero")]
    public GameObject pauseMenuPanel;

    [Header("Bottoni")]
    public GameObject logoutButton;
    
    [Header("Oggetti Audio")]
    public AudioSource backgroundMusic;

    private bool isPaused = false;

    void Start()
    {
        if(pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // 2. CONTROLLO LOGOUT:
        // Controlliamo se l'utente è loggato
        if (PlayerPrefs.HasKey("CurrentUser"))
        {
            // --- UTENTE LOGGATO (es. Andrea) ---
            Debug.Log("Utente loggato rilevato: Mostro il tasto Logout.");
            if(logoutButton != null) 
                logoutButton.SetActive(true); // MOSTRO IL BOTTONE
        }
        else
        {
            // --- NESSUNO LOGGATO (Ospite) ---
            Debug.Log("Nessun utente loggato: Nascondo il tasto Logout.");
            if(logoutButton != null) 
                logoutButton.SetActive(false); // NASCONDO IL BOTTONE
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
        // FONDAMENTALE: Prima di uscire, dobbiamo far ripartire il tempo!
        // Se non lo fai, quando ricominci a giocare il gioco sarà ancora congelato.
        Time.timeScale = 1f; 
        
        SceneManager.LoadScene("MainMenu");
    }

    //FUNZIONE 4: esce dal gioco
    public void QuitGame()
    {
        Debug.Log("Hai premuto Esci! Il gioco si sta chiudendo...");
        Application.Quit();
    }

    // FUNZIONE 5: Riavvia il livello corrente
    public void RestartLevel()
    {
        // Importante: scongela il gioco prima di ricaricare!
        Time.timeScale = 1f; 
        
        // Ricarica la scena che stai giocando adesso (resetta tutto)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // FUNZIONE VOLUME
    public void SetVolume(float volume)
    {
        // Se c'è la musica, cambia il volume
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }
    }


    public void PerformLogout()
    {
        // 1. Cancelliamo la "memoria" di chi è loggato
        PlayerPrefs.DeleteKey("CurrentUser");
        PlayerPrefs.Save();
        
        Debug.Log("Logout effettuato. Utente disconnesso.");

        // 2. Scongeliamo il tempo 
        Time.timeScale = 1f; 

        // 3. Torniamo alla Homepage
        SceneManager.LoadScene("MainMenu"); 
    }

}