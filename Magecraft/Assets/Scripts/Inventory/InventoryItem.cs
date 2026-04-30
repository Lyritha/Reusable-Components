using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMP_Text amountText;

    public void SetItem(Sprite itemIcon, int amount)
    {
        if (icon != null) icon.sprite = itemIcon;
        if (amountText != null) amountText.text = amount.ToString();
    }
}
