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

    void Awake()
    {
        // Fallback automatici per non scordare gli slot
        if (!orderUI) orderUI = GetComponentInChildren<CustomerOrderUI>(true);
        if (!database) database = FindFirstObjectByType<BobaDatabase>();
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

        // Debug utile in console
        var sb = new StringBuilder();
        sb.AppendLine($"Cliente vuole: {requestedRecipe.displayName}");
        foreach (var s in requestedRecipe.ingredients) // List<IngredientSpec>
            sb.AppendLine($"- {s.ingredient} ({s.amount})");
        Debug.Log(sb.ToString());

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
            if (req[i].amount     != del[i].amount)     return false;
        }
        return true;
    }

    private List<IngredientSpec> Normalize(List<IngredientSpec> slots)
    {
        // Confronto deterministico: ordina per nome ingrediente, poi per quantità
        return slots
            .OrderBy(s => s.ingredient)   // se è string/enum/ID va bene
            .ThenBy(s => s.amount)
            .ToList();
    }

    /// <summary>
    /// Pulisce/Nasconde la UI quando il cliente va via.
    /// </summary>
    public void ClearUI()
    {
        if (orderUI) orderUI.Hide();
    }
}
