using UnityEngine;
using TMPro;
using System.Text;

public class CustomerOrderUI : MonoBehaviour
{
    [Header("Riferimenti UI")]
    [SerializeField] private GameObject balloonRoot;   // Il contenitore della nuvoletta (pannello)
    [SerializeField] private TextMeshProUGUI recipeText;

    void Awake()
    {
        // Balloon nascosto all'avvio
        if (balloonRoot) balloonRoot.SetActive(false);
        if (recipeText) recipeText.text = "";
    }

    /// <summary>
    /// Mostra l'ordine nel balloon e lo rende visibile.
    /// </summary>
    public void ShowOrder(BobaRecipe recipe)
    {
        if (!recipe || !recipeText) return;

        var sb = new StringBuilder();
        sb.AppendLine($"<b>{recipe.displayName}</b>");
        foreach (var s in recipe.ingredients)
        {
            string level = s.amount switch { 1 => "L", 2 => "N", 3 => "X", _ => "0" };
            sb.AppendLine($"• {s.ingredient} ({level})");
        }
        recipeText.text = sb.ToString();

        if (balloonRoot) balloonRoot.SetActive(true);
    }

    /// <summary>
    /// Nasconde il balloon e pulisce il testo.
    /// </summary>
    public void Hide()
    {
        if (balloonRoot) balloonRoot.SetActive(false);
        if (recipeText) recipeText.text = "";
    }
}
