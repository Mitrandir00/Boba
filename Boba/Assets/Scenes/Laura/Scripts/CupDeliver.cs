using UnityEngine;

public class CupDeliver : MonoBehaviour
{
    [Header("Build")]
    [SerializeField] private DrinkBuilder builder;

    [Header("Drop detection")]
    [SerializeField] private LayerMask customerLayer;   
    [SerializeField] private float dropRadius = 0.35f;  

    
    public void OnConfirm()
    {
        if (!builder) return;

        var customer = FindCustomerUnderCup();
        if (customer == null) return;

        var recipe = builder.BuildRuntimeRecipe();
        if (recipe == null) return;

        customer.ReceiveDrink(recipe);  
        builder.ClearAll();             
    }

    
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
