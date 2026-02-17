using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IngredientVisualEntry
{
    public Ingredient ingredient; 
    public Sprite visualSprite;   
}

public class DrinkBuilder : MonoBehaviour
{
    [Header("Configurazione Base")]
    public string displayName = "Personalizzato";
    public Sprite icon; // Icona generica di default

    [Header("Database Ricette")]
    public BobaDatabase database; 

    [Header("Visualizzazione - Slot Ingredienti")]
    public List<SpriteRenderer> visualSlots; 
    public List<IngredientVisualEntry> visualConfig;

    [Header("Visualizzazione - Risultato Finale")]
    public SpriteRenderer finalDrinkRenderer; 
    public Sprite undefinedDrinkSprite;       

    [Header("Eventi")]
    public UnityEvent<BobaRecipe> OnBuilt; 

    // STATO INTERNO
    private List<Ingredient> _addedIngredients = new List<Ingredient>();
    private const int MAX_INGREDIENTS = 4;

    void Start()
    {
        // All'avvio nascondiamo il drink finito se è visibile
        if (finalDrinkRenderer) finalDrinkRenderer.gameObject.SetActive(false);
    }

    public int GetAmount(Ingredient ing)
    {
        return _addedIngredients.Contains(ing) ? 2 : 0;
    }

    public void Add(Ingredient ing)
    {
        // Se il drink è già shakerato (finito), resettiamo tutto se clicchi un ingrediente
        if (finalDrinkRenderer.gameObject.activeSelf)
        {
            ClearAll();
        }

        if (_addedIngredients.Contains(ing))
        {
            _addedIngredients.Remove(ing);
        }
        else
        {
            if (_addedIngredients.Count < MAX_INGREDIENTS)
            {
                _addedIngredients.Add(ing);
            }
        }
        UpdateVisuals();
    }

    // Funzione collegata al BOTTONE SHAKER
    public void Shake()
    {
        // 1. Costruiamo la ricetta attuale
        BobaRecipe currentRecipe = BuildRuntimeRecipe();
        
        // 2. Cerchiamo nel database se esiste questa combinazione
        BobaRecipe foundMatch = null;

        if (database != null)
        {
            foreach (var recipe in database.allRecipes)
            {
                var report = RecipeComparer.Compare(recipe, currentRecipe);
                if (report.isExactMatch)
                {
                    foundMatch = recipe;
                    break; 
                }
            }
        }

        // 3. Nascondiamo gli ingredienti sfusi
        foreach (var slot in visualSlots)
        {
            slot.gameObject.SetActive(false);
        }

        // 4. Mostriamo il risultato finale
        if (finalDrinkRenderer != null)
        {
            finalDrinkRenderer.gameObject.SetActive(true);

            if (foundMatch != null)
            {
                // Se esiste, mostra l'icona della ricetta
                finalDrinkRenderer.sprite = foundMatch.icon;
                Debug.Log("Drink creato: " + foundMatch.displayName);
            }
            else
            {
                // Se non esiste, mostra lo sprite "Indefinito"
                finalDrinkRenderer.sprite = undefinedDrinkSprite;
                Debug.Log("Nessuna ricetta trovata: Drink Indefinito");
            }
        }
    }

    private void UpdateVisuals()
    {
        // Se stiamo modificando gli ingredienti, nascondiamo il drink finito
        if (finalDrinkRenderer) finalDrinkRenderer.gameObject.SetActive(false);

        foreach (var slot in visualSlots)
        {
            slot.sprite = null;
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < _addedIngredients.Count; i++)
        {
            Ingredient ing = _addedIngredients[i];
            Sprite spriteToUse = GetSpriteFor(ing);

            if (spriteToUse != null && i < visualSlots.Count)
            {
                visualSlots[i].gameObject.SetActive(true);
                visualSlots[i].sprite = spriteToUse;
            }
        }
    }

    private Sprite GetSpriteFor(Ingredient ing)
    {
        foreach (var entry in visualConfig)
        {
            if (entry.ingredient == ing) return entry.visualSprite;
        }
        return null;
    }

    public void ClearAll()
    {
        _addedIngredients.Clear();
        UpdateVisuals(); // Questo nasconderà anche il finalDrinkRenderer
    }

    public BobaRecipe BuildRuntimeRecipe()
    {
        var r = ScriptableObject.CreateInstance<BobaRecipe>();
        r.id = "custom_runtime";
        r.displayName = displayName;
        r.icon = icon;
        foreach (var ing in _addedIngredients)
        {
            r.ingredients.Add(new IngredientSpec { ingredient = ing, amount = 2 });
        }
        return r;
    }

    public void Confirm()
    {
        OnBuilt?.Invoke(BuildRuntimeRecipe());
    }
}