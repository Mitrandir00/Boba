using UnityEngine;

/*
 * CupDeliver
 * - Prende la ricetta corrente dal DrinkBuilder
 * - Quando rilasci/confermi, cerca il Customer sotto la tazza e consegna
 * - Non dà feedback durante il trascinamento
 */
public class CupDeliver : MonoBehaviour
{
    [Header("Build")]
    [SerializeField] private DrinkBuilder builder;

    [Header("Drop detection")]
    [SerializeField] private LayerMask customerLayer;   // metti qui il layer "Customer"
    [SerializeField] private float dropRadius = 0.35f;  // 0.30 - 0.50 va bene

    /// <summary>
    /// Chiamalo quando FINISCI il drag / quando rilasci la tazza (o premi conferma).
    /// Se sotto c'è un cliente, consegna; altrimenti non fa nulla.
    /// </summary>
    public void OnConfirm()
    {
        if (!builder) return;

        var customer = FindCustomerUnderCup();
        if (customer == null) return;

        var recipe = builder.BuildRuntimeRecipe();
        if (recipe == null) return;

        customer.ReceiveDrink(recipe);  // il cliente controlla e reagisce
        builder.ClearAll();             // reset bicchiere
    }

    /// <summary>
    /// Bottone cestino / reset manuale.
    /// </summary>
    public void OnTrash()
    {
        if (!builder) return;
        builder.ClearAll();
    }

    private CustomerController FindCustomerUnderCup()
    {
        Vector2 p = transform.position;

        // prendi il primo collider di customer sotto alla tazza
        Collider2D hit = Physics2D.OverlapCircle(p, dropRadius, customerLayer);
        if (hit == null) return null;

        return hit.GetComponentInParent<CustomerController>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropRadius);
    }
#endif
}
