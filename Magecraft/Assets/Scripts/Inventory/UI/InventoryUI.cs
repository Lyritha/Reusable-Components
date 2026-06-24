using UnityEngine;
using UnityEngine.Rendering;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private PlayerInput input;

    [SerializeField]
    private InventoryItemUI inventoryItemUIPrefab;
    [SerializeField]
    private RectTransform inventoryRectTransform;
    [SerializeField]
    private CanvasGroup canvasGroup;

    private void Start() => Hide();
    private void OnEnable()
    {
        input.OnPerformed += Toggle;
    }
    private void OnDisable()
    {
        input.OnPerformed -= Toggle;
    }
    private void Toggle()
    {
        if (canvasGroup.alpha == 0) Show();
        else Hide();
    }
    private void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        BuildUI(true);
    }
    private void Hide()
    {
        if (Inventory.Instance != null) Inventory.Instance.Items.OnStackAdded -= AddUIItem;

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }



    private void BuildUI(bool rebuild = false)
    {
        if (rebuild) foreach (Transform child in inventoryRectTransform) Destroy(child.gameObject);

        if (Inventory.Instance == null) return;
        Inventory inventory = Inventory.Instance;

        foreach (ItemStack<ScriptableItem> item in inventory.Items.Stacks) AddUIItem(item);

        inventory.Items.OnStackAdded += AddUIItem;
    }

    private void AddUIItem(ItemStack<ScriptableItem> item)
    {
        InventoryItemUI inventoryItemUI = Instantiate(inventoryItemUIPrefab, inventoryRectTransform);
        inventoryItemUI.Initialize(item);
    }
}
