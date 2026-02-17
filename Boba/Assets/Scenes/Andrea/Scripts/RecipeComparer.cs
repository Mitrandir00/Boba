using System.Collections.Generic;
using UnityEngine;

public static class RecipeComparer
{
    public struct Report
    {
        public bool isExactMatch;
        public Dictionary<Ingredient, int> missing;     // manca proprio l'ingrediente (o livello)
        public Dictionary<Ingredient, int> extra;       // ingrediente di troppo
        public Dictionary<Ingredient, (int expected, int got)> wrongAmount; // giusto ingrediente ma livello diverso
    }


    // Confronta due BobaRecipe: expected = richiesto dal cliente, given = consegnato.
    public static Report Compare(BobaRecipe expected, BobaRecipe given)
    {
        var rep = new Report
        {
            missing = new Dictionary<Ingredient, int>(),
            extra = new Dictionary<Ingredient, int>(),
            wrongAmount = new Dictionary<Ingredient, (int, int)>()
        };

        if (expected == null || given == null)
        {
            rep.isExactMatch = false;
            return rep;
        }

        var exp = Canon(expected);
        var got = Canon(given);

        // check missing & amount differences
        foreach (var kv in exp)
        {
            var ing = kv.Key;
            var need = kv.Value;
            if (!got.TryGetValue(ing, out var have))
                rep.missing[ing] = need;
            else if (need != have)
                rep.wrongAmount[ing] = (need, have);
        }

        // check extra
        foreach (var kv in got)
        {
            if (!exp.ContainsKey(kv.Key))
                rep.extra[kv.Key] = kv.Value;
        }

        rep.isExactMatch = rep.missing.Count == 0 && rep.extra.Count == 0 && rep.wrongAmount.Count == 0;
        return rep;
    }

    private static Dictionary<Ingredient, int> Canon(BobaRecipe r)
    {
        var dict = new Dictionary<Ingredient, int>();
        if (r == null || r.ingredients == null) return dict;
        foreach (var s in r.ingredients)
        {
            if (!dict.ContainsKey(s.ingredient)) dict[s.ingredient] = 0;
            
            dict[s.ingredient] = Mathf.Clamp(Mathf.Max(dict[s.ingredient], s.amount), 0, 3);
        }
        return dict;
    }
}
