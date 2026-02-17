using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Riferimenti ai pannelli che si scambieranno
    public GameObject bg1;
    public GameObject bg2;
    
    public void SwitchToBG1()
    {
        bg2.SetActive(false);
        bg1.SetActive(true);
    }

    public void SwitchToBG2()
    {
        bg1.SetActive(false);
        bg2.SetActive(true);
    }
}