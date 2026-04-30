using UnityEngine;

[CreateAssetMenu(menuName = "Bullet/Behaviors/Explode")]
public class BulletExplode : BulletBehaviour
{
    public float radius;
    public float force;

    public override void Execute(GameObject bullet)
    {
        // explosion logic
    }
}