using UnityEngine;
using System.Text;

public class RecipeDebug : MonoBehaviour
{
    public BobaDatabase database;

    void Start()
    {
        var r = database ? database.GetRandom() : null;
        if (r == null)
        {
            Debug.LogWarning("Database vuoto o non assegnato.");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Ricetta scelta: {r.displayName}");
        foreach (var s in r.ingredients)
            sb.AppendLine($"- {s.ingredient} (lvl {s.amount})");
        Debug.Log(sb.ToString());
    }
}
