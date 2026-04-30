using UnityEngine;

[CreateAssetMenu(menuName = "Bullet/Behaviors/Homing")]
public class BulletHoming : BulletBehaviour
{
    public float turnSpeed;

    public override void Execute(GameObject bullet)
    {
        // homing logic
    }
}