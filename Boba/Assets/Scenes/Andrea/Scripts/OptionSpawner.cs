using UnityEngine;

[DisallowMultipleComponent]
public class OptionSpawner : MonoBehaviour
{
    [Header("Riferimenti")]
    public CustomerOrder targetCustomer;       // lasciarlo vuoto nel prefab
    public GameObject drinkOptionPrefab;
    public Transform spawnA;
    public Transform spawnB;

    [Header("Comportamento")]
    public bool destroyOptionsOnPick = true;

    private GameObject _optA, _optB;

    void Awake()
    {
        // Auto-wire: cerca il CustomerOrder sullo stesso prefab (parent)
        if (!targetCustomer)
            targetCustomer = GetComponentInParent<CustomerOrder>();
    }

    public void SpawnTwoOptions()
    {
        if (!targetCustomer || !targetCustomer.requestedRecipe)
        {
            Debug.LogWarning("[OptionSpawner] Target o requestedRecipe mancante. Chiama prima PickAndShowOrder().");
            return;
        }
        if (!drinkOptionPrefab || !spawnA || !spawnB)
        {
            Debug.LogWarning("[OptionSpawner] Assegna prefab e spawn points.");
            return;
        }

        ClearOptions();

        var correct = targetCustomer.requestedRecipe;
        var wrong = MakeDecoy(correct);

        bool leftIsCorrect = Random.value < 0.5f;
        var aRecipe = leftIsCorrect ? correct : wrong;
        var bRecipe = leftIsCorrect ? wrong : correct;

        _optA = Instantiate(drinkOptionPrefab, spawnA.position, spawnA.rotation);
        _optB = Instantiate(drinkOptionPrefab, spawnB.position, spawnB.rotation);

        var ca = _optA.GetComponent<ClickableDrink2D>();
        var cb = _optB.GetComponent<ClickableDrink2D>();
        if (!ca || !cb) { Debug.LogError("[OptionSpawner] Il prefab deve avere ClickableDrink2D."); return; }

        ca.recipe = aRecipe;
        cb.recipe = bRecipe;

        ca.OnClicked.AddListener(OnDrinkPicked);
        cb.OnClicked.AddListener(OnDrinkPicked);
    }

    private void OnDrinkPicked(BobaRecipe picked)
    {
        targetCustomer.ReceiveDrink(picked);
        if (destroyOptionsOnPick) ClearOptions();
    }

    public void ClearOptions()
    {
        if (_optA) Destroy(_optA);
        if (_optB) Destroy(_optB);
        _optA = _optB = null;
    }

    public BobaRecipe MakeDecoy(BobaRecipe source)
    {
        if (!source) return null;
        var decoy = ScriptableObject.CreateInstance<BobaRecipe>();
        decoy.id = source.id + "_decoy";
        decoy.displayName = source.displayName + " (decoy)";
        decoy.icon = source.icon;

        foreach (var s in source.ingredients)
            decoy.ingredients.Add(new IngredientSpec { ingredient = s.ingredient, amount = s.amount });

        if (decoy.ingredients.Count > 0)
        {
            int i = Random.Range(0, decoy.ingredients.Count);
            if (Random.value < 0.5f)
            {
                int newLevel = Random.Range(0, 4);
                if (newLevel == decoy.ingredients[i].amount)
                    newLevel = (newLevel + 1) % 4;
                decoy.ingredients[i].amount = newLevel;
            }
            else
            {
                int cur = (int)decoy.ingredients[i].ingredient;
                int last = System.Enum.GetValues(typeof(Ingredient)).Length - 1;
                int delta = Random.value < 0.5f ? -1 : 1;
                int altIndex = Mathf.Clamp(cur + delta, 0, last);
                if (altIndex != cur) decoy.ingredients[i].ingredient = (Ingredient)altIndex;
                else decoy.ingredients[i].amount = (decoy.ingredients[i].amount + 1) % 4;
            }
        }
        return decoy;
    }
}
