using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*Crea/aggiorna al volo una ricetta*/
public class DrinkBuilder : MonoBehaviour
{
    [Header("Opzionale (per UI/icon del drink creato)")]
    public string displayName = "Personalizzato";
    public Sprite icon;

    [Header("Eventi")]
    public UnityEvent<BobaRecipe> OnBuilt; // emesso quando premi Conferma

    // stato interno: quantit� 0..3 per ingrediente
    private readonly Dictionary<Ingredient, int> _amounts = new();

    public int GetAmount(Ingredient ing) => _amounts.TryGetValue(ing, out var v) ? v : 0;

    // incrementa (0..3, ciclico)
    public void Add(Ingredient ing)
    {
        int cur = GetAmount(ing);
        int next = (cur + 1) % 4; // 0->1->2->3->0
        _amounts[ing] = next;
    }

    // set esplicito (clamp 0..3)
    public void Set(Ingredient ing, int level)
    {
        _amounts[ing] = Mathf.Clamp(level, 0, 3);
    }

    public void ClearAll()
    {
        _amounts.Clear();
    }

    // Crea un BobaRecipe runtime con gli slot correnti (solo quelli >0)
    public BobaRecipe BuildRuntimeRecipe()
    {
        var r = ScriptableObject.CreateInstance<BobaRecipe>();
        r.id = "custom_runtime";
        r.displayName = displayName;
        r.icon = icon;
        foreach (var kv in _amounts)
        {
            if (kv.Value <= 0) continue;
            r.ingredients.Add(new IngredientSpec { ingredient = kv.Key, amount = Mathf.Clamp(kv.Value, 0, 3) });
        }
        return r;
    }

    // Chiamare questo dal bottone "Conferma"
    public void Confirm()
    {
        var built = BuildRuntimeRecipe();
        OnBuilt?.Invoke(built);
    }
}
