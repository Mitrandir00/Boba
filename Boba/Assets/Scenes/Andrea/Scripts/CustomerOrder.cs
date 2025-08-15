using UnityEngine;
using System.Text;

public class CustomerOrder : MonoBehaviour
{
    [Header("Data")]
    public BobaDatabase database;
    public BobaRecipe requestedRecipe { get; private set; }

    [Header("UI Hook")]
    public CustomerOrderUI orderUI;

    // Questo lo chiameremo quando il cliente è pronto a ordinare
    public void PickAndShowOrder()
    {
        requestedRecipe = database ? database.GetRandom() : null;
        if (requestedRecipe == null)
        {
            Debug.LogWarning("Nessuna ricetta trovata nel database!");
            return;
        }

        // Debug in console
        var sb = new StringBuilder();
        sb.AppendLine($"Cliente vuole: {requestedRecipe.displayName}");
        foreach (var s in requestedRecipe.ingredients)
            sb.AppendLine($"- {s.ingredient} ({s.amount})");
        Debug.Log(sb.ToString());

        // Mostra nella UI
        if (orderUI) orderUI.ShowOrder(requestedRecipe);
    }
}
