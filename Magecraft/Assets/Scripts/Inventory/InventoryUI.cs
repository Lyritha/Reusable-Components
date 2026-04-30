using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private Transform inventoryItemsParent;
    [SerializeField]
    private InventoryItem itemPrefab;

    private void Awake()
    {
        inventory.BulletPouch.OnPouchUpdated += UpdateBulletUI;

    }

    private void UpdateBulletUI(Dictionary<ScriptableBullets, int> dictionary)
    {
        foreach (Transform child in inventoryItemsParent)
            Destroy(child.gameObject);

        foreach (var item in dictionary)
        {
            InventoryItem inventoryItem = Instantiate(itemPrefab, inventoryItemsParent);
            inventoryItem.SetItem(item.Key.Icon, item.Value);
        }
    }
}
