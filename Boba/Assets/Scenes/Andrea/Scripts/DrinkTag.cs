using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DrinkTag : MonoBehaviour
{
    public bool isCorrect; // true = drink richiesto, false = decoy

    public void ApplyDebugStyle()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (!sr) return;

        // verde per corretto, rosso per sbagliato (solo per debug)
        sr.color = isCorrect ? new Color(0.6f, 1f, 0.6f, 1f) : new Color(1f, 0.6f, 0.6f, 1f);

        // rinomina in Hierarchy (utile)
        gameObject.name = isCorrect ? "Drink_CORRETTO" : "Drink_SBAGLIATO";
    }
}