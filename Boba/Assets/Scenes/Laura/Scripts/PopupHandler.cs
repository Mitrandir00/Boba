using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    public GameObject popupImage; 

    void Start()
    {
        if(popupImage != null) popupImage.SetActive(false);
    }

    public void MostraPopup()
    {
        popupImage.SetActive(true);
    }

    public void NascondiPopup()
    {
        popupImage.SetActive(false);
    }
}