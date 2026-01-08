using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* Un bottone per ogni ingrediente */
public class IngredientButton : MonoBehaviour
{
    public DrinkBuilder builder;
    public Ingredient ingredient;
    public TextMeshProUGUI levelText; // opzionale, per mostrare L/N/X o '-'

    // --- QUESTA È L'UNICA PARTE MODIFICATA ---
    public void OnClick()
    {
        if (!builder) return;

        // 1. Capiamo se stai Aggiungendo (costa) o Rimuovendo (gratis)
        // Usiamo la tua funzione GetAmount: se è 0 vuol dire che non c'è, quindi stai aggiungendo.
        bool isAdding = builder.GetAmount(ingredient) == 0;

        if (isAdding)
        {
            // --- FASE PAGAMENTO ---
            // Recupera il costo dal manager
            int cost = 0;
            if (EconomyManager.instance != null)
            {
                cost = EconomyManager.instance.GetIngredientCost(ingredient);
            }

            // Se EconomyManager esiste, prova a pagare. 
            // SpendCoins scala i soldi e ritorna TRUE. Se non hai soldi ritorna FALSE.
            if (EconomyManager.instance != null)
            {
                if (EconomyManager.instance.SpendCoins(cost))
                {
                    // Ha pagato: Procedi con la logica originale
                    builder.Add(ingredient);
                }
                else
                {
                    // Non ha pagato: Stop. Non aggiunge nulla.
                    Debug.Log("Soldi insufficienti per: " + ingredient);
                    return; 
                }
            }
            else
            {
                // Se non c'è l'EconomyManager (es. test rapidi), funziona come prima (gratis)
                builder.Add(ingredient);
            }
        }
        else
        {
            // --- FASE RIMOZIONE ---
            // Se l'ingrediente c'era già, lo togliamo gratis.
            // La tua funzione builder.Add gestisce già la rimozione se l'ingrediente è presente.
            builder.Add(ingredient);
        }

        // Aggiorna la grafica (Codice originale)
        Refresh();
    }
    // -----------------------------------------

    // parte per i soldi
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