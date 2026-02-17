using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientButton : MonoBehaviour
{
    public DrinkBuilder builder;
    public Ingredient ingredient;
    public TextMeshProUGUI levelText;

    
    public void OnClick()
    {
        if (!builder) return;

        bool isAdding = builder.GetAmount(ingredient) == 0;

        if (isAdding)
        {
            int cost = 0;
            if (EconomyManager.instance != null)
            {
                cost = EconomyManager.instance.GetIngredientCost(ingredient);
            }

            if (EconomyManager.instance != null)
            {
                if (EconomyManager.instance.SpendCoins(cost))
                {
                    builder.Add(ingredient);
                }
                else
                {
                    Debug.Log("Soldi insufficienti per: " + ingredient);
                    return; 
                }
            }
            else
            {
                builder.Add(ingredient);
            }
        }
        else
        {
            builder.Add(ingredient);
        }
        Refresh();
    }
    
    public void Refresh()
    {
        if (!levelText || !builder) return;
        int lvl = builder.GetAmount(ingredient);
        
        string txt = lvl switch { 0 => "-", 1 => "L", 2 => "N", 3 => "X", _ => lvl.ToString() };
        levelText.text = txt;
    }

    private void OnEnable() => Refresh();
}