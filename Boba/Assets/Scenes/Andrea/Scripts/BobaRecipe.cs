using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BobaRecipe", menuName = "Boba/BobaRecipe")]
public class BobaRecipe : ScriptableObject
{
    public string id;          // es. "classic_milk_tea"
    public string displayName; // es. "Classic Milk Tea"
    public Sprite icon;        // la useremo dopo per il balloon
    
    [TextArea(3, 5)] // Crea un box di testo più grande nell'Inspector
    public string storyDialogue; // ES: "Vorrei un tè al latte classico, grazie!"
    public List<IngredientSpec> ingredients = new();
    
    public Color indicatorColor = Color.white; // colore del quadrato / tint dell’icona

}
