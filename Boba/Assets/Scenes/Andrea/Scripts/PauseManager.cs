using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Trascina qui l'oggetto 'PauseMenu' intero")]
    public GameObject pauseMenuPanel;

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

    //FUNZIONE 3: Apre le opzioni
    public void OpenOptions()
    {
        Debug.Log("Qui si apriranno le opzioni!"); 
    }

    //FUNZIONE 4: esce dal gioco
    public void QuitGame()
    {
        Debug.Log("Hai premuto Esci! Il gioco si sta chiudendo...");
        Application.Quit();
    }
}