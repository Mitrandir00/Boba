using UnityEngine;
/*Sostituisce i vecchi SI/NO al click su conferma prende 
la ricetta dal DrinkBuilder e la consegna al cliente in attesa*/

public class CupDeliver : MonoBehaviour
{
    public DrinkBuilder builder;
    public CustomerController targetCustomer; // assegnalo a runtime o in scena

    public void OnConfirm()
    {
        if (!builder || !targetCustomer) return;
        var recipe = builder.BuildRuntimeRecipe();
        targetCustomer.ReceiveDrink(recipe); // mostra SI/NO e fa uscire il cliente
        builder.ClearAll();                  // reset bicchiere
    }

    public void OnTrash() // bottone cestino/reset
    {
        if (!builder) return;
        builder.ClearAll();
    }
}
