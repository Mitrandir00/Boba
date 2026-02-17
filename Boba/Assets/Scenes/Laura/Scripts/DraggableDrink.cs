using UnityEngine;

[RequireComponent(typeof(Collider2D))] 
public class DraggableDrink : MonoBehaviour
{
    public DrinkBuilder builder; 
    
    private Vector3 startPosition;

    void OnEnable()
    {
        startPosition = transform.position;
    }


    void OnMouseDrag()
    {
        // Converte la posizione del mouse dallo schermo al mondo di gioco
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; //deve rimanere in 2D
        transform.position = mousePos;
    }

    void OnMouseUp()
    {
        CheckDrop();
    }

    private void CheckDrop()
    {
        // Controlla se nel punto in cui abbiamo lasciato il drink c'è un Cliente
        // Usa un raggio puntiforme
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);

        foreach (var hit in hits)
        {
            // Cerca lo script del cliente nell'oggetto colpito
            CustomerController customer = hit.GetComponent<CustomerController>();
            
            // Se troviamo un cliente
            if (customer != null)
            {
                DeliverTo(customer);
                return; 
            }
        }

        // Se non abbiamo colpito nessun cliente, il drink torna al posto di partenza
        transform.position = startPosition;
    }

    private void DeliverTo(CustomerController customer)
    {
        // 1. Chiediamo al Builder di costruire la ricetta corrente
        BobaRecipe recipe = builder.BuildRuntimeRecipe();

        // 2. La consegniamo al cliente 
        customer.ReceiveDrink(recipe);

        // 3. Nascondiamo il drink 
        gameObject.SetActive(false);

        // 4. Puliamo il builder per il prossimo ordine
        builder.ClearAll();
        
        // Rimettiamo l'oggetto nella posizione originale per il futuro
        transform.position = startPosition;
    }
}