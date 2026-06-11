using System.Linq;
using UnityEngine;

public class Inventory : IdentifiableBehaviour<Inventory>
{
    private ItemPouch<ScriptableItem> inventoryPouch = new();

    private ItemPouch<Bullet> bullets = new();

    public ItemStack<ScriptableItem>[] GetItemsOfType(ItemType type)
    {
        return inventoryPouch.AllStacks.Where(stack => stack.Item.type == type).ToArray();
    }

    public ItemPouch<ScriptableItem> InventoryPouch => inventoryPouch;
    public ItemPouch<Bullet> Bullets => bullets;
}
