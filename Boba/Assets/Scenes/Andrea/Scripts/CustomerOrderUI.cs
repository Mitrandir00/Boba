using UnityEngine;
using TMPro;
using System.Text;

public class CustomerOrderUI : MonoBehaviour
{
    [Header("Modalità Infinita (Icone)")]
    public GameObject iconPanel;

    [Header("Modalità Storia (Balloon)")]
    public GameObject balloonPanel;
    public TextMeshProUGUI dialogueText; 

    [Header("Balloon / UI")]
    [SerializeField] private GameObject balloonRoot;        
    [SerializeField] private TextMeshProUGUI recipeText;


    public void ShowVisualOrder(BobaRecipe recipe)
    {
        // Attiva icona, disattiva balloon
        if(iconPanel) iconPanel.SetActive(true);
        if(balloonPanel) balloonPanel.SetActive(false);
    }
    public void ShowTextOrder(string text)
    {
        // Disattiva icona, attiva balloon
        if(iconPanel) iconPanel.SetActive(false);
        if(balloonPanel) balloonPanel.SetActive(true);

        if (dialogueText != null)
        {
            dialogueText.text = text;
        }
    }

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

    public void ShowYesNo(bool correct)
    {
        if (!recipeText) return;
        recipeText.text = correct ? "YEAHHH" : "NOPE";
        balloonRoot.SetActive(true);
    }

    public void Stufo()
    {
        recipeText.text = "UFFA";
        balloonRoot.SetActive(true);
    }
    public void Hide()
    {
        if(iconPanel) iconPanel.SetActive(false);
        if(balloonPanel) balloonPanel.SetActive(false);
    }
    
    // Quando il cliente (il padre) viene distrutto, Unity chiama questo metodo.
    private void OnDestroy()
    {
        Hide();
    }
}
