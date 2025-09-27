using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*Un bottone per ogni ingrediente*/
public class IngredientButton : MonoBehaviour
{
    public DrinkBuilder builder;
    public Ingredient ingredient;
    public TextMeshProUGUI levelText; // opzionale, per mostrare L/N/X o '-'

    public void OnClick()
    {
        if (!builder) return;
        builder.Add(ingredient);
        Refresh();
    }

    public void Refresh()
    {
        if (!levelText || !builder) return;
        int lvl = builder.GetAmount(ingredient);
        // mappa come in CustomerOrderUI
        string txt = lvl switch { 0 => "-", 1 => "L", 2 => "N", 3 => "X", _ => lvl.ToString() };
        levelText.text = txt;
    }

    private void OnEnable() => Refresh();
}
