using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private ItemPouch<ScriptableItem> inventoryPouch;

    public ItemStack<ScriptableItem>[] GetItemsOfType(ItemType type)
    {
        return inventoryPouch.AllStacks.Where(stack => stack.Item.type == type).ToArray();
    }
}
