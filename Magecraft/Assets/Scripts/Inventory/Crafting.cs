using UnityEngine;

public class Crafting : MonoBehaviour
{
    [SerializeField]
    private ScriptableBullets[] craftingRecipes;

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private RectTransform craftButtonsParent;
    [SerializeField]
    private SimpleButton craftButtonPrefab;

    private void Awake()
    {
        foreach (var b in craftingRecipes)
        {
            ScriptableBullets recipe = b; // capture correctly

            SimpleButton button = Instantiate(craftButtonPrefab, craftButtonsParent);
            button.SetTitle(recipe.Name);
            button.Button.onClick.AddListener(() => Craft(recipe));
        }

    }

    private void Craft(ScriptableBullets recipe)
    {
        inventory.BulletPouch.AddItems(recipe, 1);
        Debug.Log("Crafted " + recipe.Name);
    }   
}
