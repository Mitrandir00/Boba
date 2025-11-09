using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject optionsPanel;
    
    [Header("Audio Sliders")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    
    void Start()
    {
        // Mostra menu principale all'avvio
        ShowMainMenu();
        
        // Carica volumi salvati
        LoadVolumeSettings();
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
    
    public void LoadScene(string main)  //vabbè ovviamente anche qui
    {
        SceneManager.LoadScene(main); //QUI VA AGGIUNTO IL NOME DELLA SCENA DA CARICARE
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
    
    // === GESTIONE AUDIO ===
    
    public void OnMusicVolumeChanged()
    {
        if (musicVolumeSlider != null)
        {
            float volume = musicVolumeSlider.value;
            PlayerPrefs.SetFloat("MusicVolume", volume);
            Debug.Log("Volume musica: " + (volume * 100) + "%");
        }
    }
    
    public void OnSFXVolumeChanged()
    {
        if (sfxVolumeSlider != null)
        {
            float volume = sfxVolumeSlider.value;
            PlayerPrefs.SetFloat("SFXVolume", volume);
            Debug.Log("Volume effetti: " + (volume * 100) + "%");
        }
    }
    
    void LoadVolumeSettings()
    {
        if (musicVolumeSlider != null)
        {
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            musicVolumeSlider.value = musicVol;
        }
        
        if (sfxVolumeSlider != null)
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
            sfxVolumeSlider.value = sfxVol;
        }
    }
}
