using UnityEngine;

[CreateAssetMenu(fileName = "DefaultItem", menuName = "Items/DefaultItem")]
public partial class ScriptableItem : ScriptableObject
{
    [Header("Item Info")]
    public string Name;
    public Sprite Icon;
    public ItemType type;
}
