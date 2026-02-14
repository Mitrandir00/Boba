using UnityEngine;
using System.Collections.Generic;

public class SpoiledCatNoPay : MonoBehaviour
{
    [Header("Story only + 1 volta per livello")]
    public bool storyOnly = true;
    public bool onlyOncePerLevel = false;

    private static HashSet<int> usedLevels = new HashSet<int>();

    private void OnEnable()
    {
        if (storyOnly && !GameSettings.IsStoryMode)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        int lvl = Mathf.Clamp(GameSettings.SelectedLevel, 1, 3);

        if (!onlyOncePerLevel) return;

        if (usedLevels.Contains(lvl))
        {
            // Se per errore è stato messo due volte in customerSequence
            Destroy(gameObject);
            return;
        }

        usedLevels.Add(lvl);
    }

    public static void ResetRunMemory()
    {
        usedLevels.Clear();
    }
}
