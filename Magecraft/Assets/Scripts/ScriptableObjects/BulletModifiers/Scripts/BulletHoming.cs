using UnityEngine;

[CreateAssetMenu(menuName = "Bullet/Modifiers/Homing")]
public class BulletHoming : BulletModifier
{
    [Header("Behavior")]
    public float turnSpeed;

    public override bool HasExecutable => true;
    public override void Execute(GameObject bullet)
    {
        // homing logic
    }
}