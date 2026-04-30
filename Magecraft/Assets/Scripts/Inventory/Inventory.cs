using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private InventoryPouch<ScriptableBullets> bulletPouch = new();
    private InventoryPouch<ScriptableItem> itemPouch = new();

    public InventoryPouch<ScriptableBullets> BulletPouch => bulletPouch;
    public InventoryPouch<ScriptableItem> ItemPouch => itemPouch;
}
