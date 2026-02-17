using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BobaRecipe", menuName = "Boba/BobaRecipe")]
public class BobaRecipe : ScriptableObject
{
    public string id;         
    public string displayName; 
    public Sprite icon;        
    
    [TextArea(3, 5)]
    public string storyDialogue; 
    public List<IngredientSpec> ingredients = new();
    
    public Color indicatorColor = Color.white; 

}
