using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ClickableDrink2D : MonoBehaviour
{
    [Tooltip("La ricetta che questo 'boba' rappresenta")]
    public BobaRecipe recipe;

    [Tooltip("Emesso quando clicchi questo boba")]
    public UnityEvent<BobaRecipe> OnClicked;

    private bool _consumed; // evita doppi click

    // Meglio di OnMouseDown in 2D per evitare drag accidentali
    void OnMouseUpAsButton()
    {
        if (_consumed) return;
        _consumed = true;
        OnClicked?.Invoke(recipe);
    }
}
