using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BobaRecipe", menuName = "Boba/BobaRecipe")]
public class BobaRecipe : ScriptableObject
{
    public string id;          // es. "classic_milk_tea" ppppp
    public string displayName; // es. "Classic Milk Tea"
    public Sprite icon;        // la useremo dopo per il balloon
    public List<IngredientSpec> ingredients = new();
    
    public Color indicatorColor = Color.white; // colore del quadrato / tint dell’icona

}
