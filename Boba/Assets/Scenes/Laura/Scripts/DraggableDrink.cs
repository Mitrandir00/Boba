using UnityEngine;

[RequireComponent(typeof(Collider2D))] // Obbliga ad avere un collider per funzionare
public class DraggableDrink : MonoBehaviour
{
    public DrinkBuilder builder; // Riferimento per sapere CHE COSA stiamo consegnando
    
    private Vector3 startPosition;


    // Quando l'oggetto si attiva (dopo lo Shaker), salviamo la posizione iniziale
    void OnEnable()
    {
        startPosition = transform.position;
    }


    void OnMouseDrag()
    {
        // Converte la posizione del mouse dallo schermo al mondo di gioco
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Assicuriamoci che rimanga in 2D
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
            
            // Se troviamo un cliente (e non stiamo trascinando su noi stessi)
            if (customer != null)
            {
                DeliverTo(customer);
                return; // Fermati, consegna effettuata
            }
        }

        // Se non abbiamo colpito nessun cliente, il drink torna al posto di partenza
        transform.position = startPosition;
    }

    private void DeliverTo(CustomerController customer)
    {
        // 1. Chiediamo al Builder di costruire la ricetta corrente
        BobaRecipe recipe = builder.BuildRuntimeRecipe();

        // 2. La consegniamo al cliente (usando il tuo metodo esistente)
        customer.ReceiveDrink(recipe);

        // 3. Nascondiamo il drink (è stato consegnato)
        gameObject.SetActive(false);

        // 4. Puliamo il builder per il prossimo ordine
        builder.ClearAll();
        
        // (Opzionale) Rimettiamo l'oggetto nella posizione originale per il futuro
        transform.position = startPosition;
    }
}