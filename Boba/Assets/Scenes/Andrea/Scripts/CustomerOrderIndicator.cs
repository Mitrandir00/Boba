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

        if (recipe.icon != null)
            sr.sprite = recipe.icon;

        sr.color = recipe.indicatorColor;

        sr.enabled = true;
    }

    public void Hide()
    {
        if (sr) sr.enabled = false;
    }
}
