using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Trascina qui l'oggetto 'PauseMenu' intero")]
    public GameObject pauseMenuPanel;
    
    [Header("Oggetti Audio")]
    public AudioSource backgroundMusic;

    private bool isPaused = false;

    void Start()
    {
        // il menu chiuso quando parte il gioco
        if(pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
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

    // --- NUOVA FUNZIONE: IL VOLUME ---
    public void SetVolume(float volume)
    {
        // Se c'è la musica, cambia il volume
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }
    }
}