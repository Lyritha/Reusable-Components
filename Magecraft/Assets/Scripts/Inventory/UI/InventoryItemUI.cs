using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMP_Text itemTitle;
    [SerializeField]
    private TMP_Text itemCount;

    public void Initialize(ItemStack<ScriptableItem> stack)
    {
        stack.OnChanged += UpdateUI;
        stack.OnDestroyed += DestroyUI;

        UpdateUI(stack);
    }

    private void UpdateUI(ItemStack<ScriptableItem> stack)
    {
        icon.sprite = stack.Item.Icon;
        itemTitle.text = stack.Item.Name;
        itemCount.text = $"Count: {stack.Count}";
    }

    private void DestroyUI(ItemStack<ScriptableItem> stack)
    {
        stack.OnChanged -= UpdateUI;
        stack.OnDestroyed -= DestroyUI;

        Destroy(gameObject);
    }
}
