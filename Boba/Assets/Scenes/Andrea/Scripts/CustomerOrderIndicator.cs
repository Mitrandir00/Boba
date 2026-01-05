using UnityEngine;

public class CustomerOrderIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;

    private void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        Hide(); // parte nascosto
    }

    public void Show(BobaRecipe recipe)
    {
        if (!sr || recipe == null) return;

        // Se in futuro la tua amica mette la nuvoletta in recipe.icon, qui la useremo
        if (recipe.icon != null)
            sr.sprite = recipe.icon;

        // Oggi: quadrato bianco + colore
        sr.color = recipe.indicatorColor;

        sr.enabled = true;
    }

    public void Hide()
    {
        if (sr) sr.enabled = false;
    }
}
