using UnityEngine;
using TMPro;
using System.Text;

public class CustomerOrderUI : MonoBehaviour
{
    [Header("Balloon / UI")]
    [SerializeField] private GameObject balloonRoot;        
    [SerializeField] private TextMeshProUGUI recipeText;

    void Awake()
    {
        if (!balloonRoot) balloonRoot = gameObject;
        if (!recipeText)  recipeText  = GetComponentInChildren<TextMeshProUGUI>(true);

        balloonRoot.SetActive(false);
        if (recipeText) recipeText.text = "";
    }

    public void ShowOrder(BobaRecipe recipe)
    {
        if (recipe == null) return;

        if (recipeText)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{recipe.displayName}</b>");
            foreach (var s in recipe.ingredients)
            {
                string level = s.amount switch { 1 => "L", 2 => "N", 3 => "X", _ => s.amount.ToString() };
                sb.AppendLine($"• {s.ingredient} ({level})");
            }
            recipeText.text = sb.ToString();
        }

        balloonRoot.SetActive(true);
    }

    /// <summary>
    /// Mostra "SI" o "NO" nel balloon.
    /// </summary>
    public void ShowYesNo(bool correct)
    {
        if (!recipeText) return;
        recipeText.text = correct ? "SI" : "NO";
        balloonRoot.SetActive(true);
    }

    public void Stufo()
    {
        recipeText.text = "UFFA";
        balloonRoot.SetActive(true);
    }
    public void Hide()
    {
        balloonRoot.SetActive(false);
        if (recipeText) recipeText.text = "";
    }
}
