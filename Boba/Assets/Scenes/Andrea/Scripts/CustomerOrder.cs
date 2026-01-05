using UnityEngine;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public class CustomerOrder : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] public BobaDatabase database;
    public BobaRecipe requestedRecipe { get; private set; }

    [Header("UI Hook")]
    [SerializeField] public CustomerOrderUI orderUI;

    [Header("Indicator (quadrato/nuvoletta sopra la testa)")]
    [SerializeField] private CustomerOrderIndicator indicator;

    [Header("Eventi")]
    public UnityEngine.Events.UnityEvent OnCorrectDrink;
    public UnityEngine.Events.UnityEvent OnWrongDrink;

    void Awake()
    {
        // Fallback automatici per non scordare gli slot
        if (!orderUI) orderUI = GetComponentInChildren<CustomerOrderUI>(true);

        // ⚠️ BobaDatabase è un ScriptableObject, quindi FindFirstObjectByType NON lo trova.
        // Ti conviene assegnarlo da Inspector.
        // Se vuoi un fallback, usa una reference via inspector o un manager.
        // Qui lascio il tuo comportamento ma con un warning più chiaro:
        if (!database)
        {
            Debug.LogWarning("[CustomerOrder] Database non assegnato! Trascina BobaDatabase nell'Inspector.");
        }

        if (!indicator) indicator = GetComponentInChildren<CustomerOrderIndicator>(true);
    }

    /// <summary>
    /// Chiamato quando il cliente è pronto a ordinare (collega a OnReadyToOrder).
    /// </summary>
    public void PickAndShowOrder()
    {
        requestedRecipe = database ? database.GetRandom() : null;
        if (requestedRecipe == null)
        {
            Debug.LogWarning("[CustomerOrder] Nessuna ricetta trovata nel database!");
            return;
        }

        // ✅ ACCENDE L’INDICATORE (oggi quadrato colorato, domani sprite nuvoletta)
        if (indicator) indicator.Show(requestedRecipe);

        // Debug utile in console
        var sb = new StringBuilder();
        sb.AppendLine($"Cliente vuole: {requestedRecipe.displayName} ({requestedRecipe.id})");
        foreach (var s in requestedRecipe.ingredients)
            sb.AppendLine($"- {s.ingredient} ({s.amount})");
        Debug.Log(sb.ToString());

        // Balloon/ordine (se già lo usi)
        if (orderUI) orderUI.ShowOrder(requestedRecipe);
    }

    /// <summary>
    /// Confronto "esatto" di ricette: stessi ingredienti e stesse quantità (ordine indipendente).
    /// </summary>
    public bool Matches(BobaRecipe delivered)
    {
        if (requestedRecipe == null || delivered == null) return false;

        var req = Normalize(requestedRecipe.ingredients);
        var del = Normalize(delivered.ingredients);

        if (req.Count != del.Count) return false;

        for (int i = 0; i < req.Count; i++)
        {
            if (req[i].ingredient != del[i].ingredient) return false;
            if (req[i].amount != del[i].amount) return false;
        }
        return true;
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
        var report = RecipeComparer.Compare(requestedRecipe, delivered);
        if (report.isExactMatch)
            OnCorrectDrink?.Invoke();
        else
            OnWrongDrink?.Invoke();
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
