using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Riferimenti ai pannelli che si scambieranno
    public GameObject bg1;
    public GameObject bg2;
    
    // Funzione chiamata dal pulsante per passare agli ingredienti
    public void SwitchToBG1()
    {
        // Disattiva il pannello del caffè
        bg2.SetActive(false);
        // Attiva il pannello degli ingredienti
        bg1.SetActive(true);
    }

    // Funzione chiamata dal pulsante per passare al caffè
    public void SwitchToBG2()
    {
        // Disattiva il pannello degli ingredienti
        bg1.SetActive(false);
        // Attiva il pannello del caffè
        bg2.SetActive(true);
    }
}