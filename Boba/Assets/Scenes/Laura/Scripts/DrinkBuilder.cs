using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IngredientVisualEntry
{
    public Ingredient ingredient; // L'ingrediente (Enum)
    public Sprite visualSprite;   // L'immagine da mostrare nello slot
}

public class DrinkBuilder : MonoBehaviour
{
    [Header("Configurazione")]
    public string displayName = "Personalizzato";
    public Sprite icon;

    [Header("Visualizzazione - Slot")]
    // Trascina qui i 4 GameObjects con SpriteRenderer che hai creato (Slot1, Slot2...)
    public List<SpriteRenderer> visualSlots; 

    // Qui associ: Enum -> Sprite (es. Tapioca -> Immagine palline)
    public List<IngredientVisualEntry> visualConfig;

    [Header("Eventi")]
    public UnityEvent<BobaRecipe> OnBuilt; 

    // STATO INTERNO
    // Usiamo una Lista per mantenere l'ordine di inserimento
    private List<Ingredient> _addedIngredients = new List<Ingredient>();
    private const int MAX_INGREDIENTS = 4;

    // Helper per ottenere l'amount (usato dai bottoni per mostrare i livelli)
    public int GetAmount(Ingredient ing)
    {
        // Se l'ingrediente è nella lista, ritorniamo 2 (Normal), altrimenti 0
        return _addedIngredients.Contains(ing) ? 2 : 0;
    }

    public void Add(Ingredient ing)
    {
        // 1. Se l'ingrediente è già presente, lo RIMUOVIAMO (Toggle Off)
        if (_addedIngredients.Contains(ing))
        {
            _addedIngredients.Remove(ing);
        }
        else
        {
            // 2. Se non c'è, controlliamo se c'è spazio (Max 4)
            if (_addedIngredients.Count < MAX_INGREDIENTS)
            {
                _addedIngredients.Add(ing);
            }
            else
            {
                Debug.Log("Bicchiere pieno! Massimo 4 ingredienti.");
                return; // Esce senza fare nulla
            }
        }

        // 3. Aggiorniamo la grafica
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // A. Resettiamo tutti gli slot (li svuotiamo)
        foreach (var slot in visualSlots)
        {
            slot.sprite = null;
            slot.gameObject.SetActive(false);
        }

        // B. Riempiamo gli slot in base all'ordine della lista
        for (int i = 0; i < _addedIngredients.Count; i++)
        {
            Ingredient ing = _addedIngredients[i];
            
            // Cerchiamo lo sprite giusto nella configurazione
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
        UpdateVisuals();
    }

    // Costruisce la ricetta finale per il cliente
    public BobaRecipe BuildRuntimeRecipe()
    {
        var r = ScriptableObject.CreateInstance<BobaRecipe>();
        r.id = "custom_runtime";
        r.displayName = displayName;
        r.icon = icon;

        // Convertiamo la lista ordinata nel formato richiesto dalla ricetta
        foreach (var ing in _addedIngredients)
        {
            // Aggiungiamo con quantità 2 (Normal) come standard
            r.ingredients.Add(new IngredientSpec { ingredient = ing, amount = 2 });
        }
        return r;
    }

    public void Confirm()
    {
        var built = BuildRuntimeRecipe();
        OnBuilt?.Invoke(built);
    }
}