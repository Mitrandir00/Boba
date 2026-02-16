using UnityEngine;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public class CustomerOrder : MonoBehaviour
{
    [Header("Impostazioni Generali")]
    public BobaDatabase database;
    [SerializeField] public CustomerOrderUI orderUI;
    [SerializeField] private CustomerOrderIndicator indicator;

    [Header("--- SOLO PER MODALITÀ STORIA ---")]
    [Tooltip("Trascina qui la ricetta specifica che questo cliente deve ordinare")]
    public BobaRecipe storyRecipe; 

    [TextArea(3, 5)]
    [Tooltip("Scrivi qui la frase che apparirà nel balloon")]
    public string storyDialogue; 
    // ------------------------------------

    // La ricetta che il gioco userà effettivamente
    public BobaRecipe requestedRecipe { get; private set; }

    [Header("Eventi")]
    public UnityEngine.Events.UnityEvent OnCorrectDrink;
    public UnityEngine.Events.UnityEvent OnWrongDrink;

    void Awake()
    {
        if (!orderUI) orderUI = GetComponentInChildren<CustomerOrderUI>(true);
        if (!indicator) indicator = GetComponentInChildren<CustomerOrderIndicator>(true);
    }

    /// <summary>
    /// Chiamato quando il cliente è pronto a ordinare (collega a OnReadyToOrder).
    /// </summary>
    public void PickAndShowOrder()
    {
        // 1. SELEZIONE DELLA RICETTA
        if (GameSettings.IsStoryMode && storyRecipe != null)
        {
            // Se siamo nella storia e hai impostato una ricetta specifica, usa quella
            requestedRecipe = storyRecipe;
        }
        else
        {
            // Altrimenti (infinita o se non hai messo nulla) ne pesca una a caso
            requestedRecipe = database ? database.GetRandom() : null;
        }

        if (requestedRecipe == null)
        {
            Debug.LogWarning($"[CustomerOrder] Nessuna ricetta trovata per {gameObject.name}!");
            return;
        }

        // 2. VISUALIZZAZIONE (Balloon vs Icona)
        if (GameSettings.IsStoryMode)
        {
            // Nascondi l'icona sopra la testa
            if (indicator) indicator.Hide();

            // Scegli il testo: usa quello personalizzato se c'è, altrimenti un fallback
            string textToShow = !string.IsNullOrEmpty(storyDialogue) 
                                ? storyDialogue 
                                : "Vorrei un " + requestedRecipe.displayName;

            // Mostra il balloon
            if (orderUI) orderUI.ShowTextOrder(textToShow);
        }
        else
        {
            // Modalità infinita: mostra l'icona
            if (indicator) indicator.Show(requestedRecipe);
            if (orderUI) orderUI.ShowVisualOrder(requestedRecipe);
        }
    }
    // --- Logica di confronto e consegna (rimane uguale) ---
    public bool Matches(BobaRecipe delivered)
    {
        if (requestedRecipe == null || delivered == null) return false;
        var rep = RecipeComparer.Compare(requestedRecipe, delivered);
        return rep.isExactMatch;
    }

    private List<IngredientSpec> Normalize(List<IngredientSpec> slots)
    {
        var agg = new Dictionary<Ingredient, int>();
        foreach (var s in slots)
        {
            if (!agg.ContainsKey(s.ingredient)) agg[s.ingredient] = 0;
            agg[s.ingredient] = Mathf.Clamp(agg[s.ingredient] + s.amount, 0, 3);
        }

        return agg.Select(kv => new IngredientSpec { ingredient = kv.Key, amount = kv.Value })
                  .OrderBy(s => s.ingredient)
                  .ToList();
    }

    public void ReceiveDrink(BobaRecipe delivered)
    {
        if (Matches(delivered)) OnCorrectDrink?.Invoke();
        else OnWrongDrink?.Invoke();
    }

    /// <summary>
    /// Pulisce/Nasconde la UI quando il cliente va via.
    /// </summary>
    public void ClearUI()
    {
        if (orderUI) orderUI.Hide();
        if (indicator) indicator.Hide();
    }
}
