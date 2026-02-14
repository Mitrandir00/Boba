using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    public GameObject popupImage; // Trascina qui l'immagine che deve apparire

    void Start()
    {
        // Assicuriamoci che sia chiusa all'inizio
        if(popupImage != null) popupImage.SetActive(false);
    }

    // Funzione per il bottone principale
    public void MostraPopup()
    {
        popupImage.SetActive(true);
    }

    // Funzione per quando clicchi l'immagine stessa
    public void NascondiPopup()
    {
        popupImage.SetActive(false);
    }
}