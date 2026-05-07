using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Bullet", menuName = "Items/Bullet")]
public class ScriptableBullets : ScriptableItem
{
    [Header("Bullet Properties")]
    public int DamageAmount;

    // List of modifiers that apply to this bullet, such as homing, exploding, etc.
    public BulletModifier[] modifier;

    private void OnValidate()
    {
        type = ItemType.Bullet;
    }
}
