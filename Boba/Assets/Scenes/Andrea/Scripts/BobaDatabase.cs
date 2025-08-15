using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BobaDatabase", menuName = "Boba/BobaDatabase")]
public class BobaDatabase : ScriptableObject
{
    public List<BobaRecipe> allRecipes = new();

    public BobaRecipe GetRandom()
    {
        if (allRecipes == null || allRecipes.Count == 0) return null;
        return allRecipes[Random.Range(0, allRecipes.Count)];
    }
}
