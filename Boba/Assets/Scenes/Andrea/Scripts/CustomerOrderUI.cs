using UnityEngine;
using TMPro;
using System.Text;

public class CustomerOrderUI : MonoBehaviour
{
    public TextMeshProUGUI recipeText;

    public void ShowOrder(BobaRecipe recipe)
    {
        if (!recipeText || recipe == null) return;

        var sb = new StringBuilder();
        sb.AppendLine($"<b>{recipe.displayName}</b>");
        foreach (var s in recipe.ingredients)
        {
            string level = s.amount switch { 1 => "L", 2 => "N", 3 => "X", _ => "0" };
            sb.AppendLine($"• {s.ingredient} ({level})");
        }
        recipeText.text = sb.ToString();
    }
}
