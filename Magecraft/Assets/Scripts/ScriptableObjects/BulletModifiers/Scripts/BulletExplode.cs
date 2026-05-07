using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Bullet/Modifiers/Explode")]
public class BulletExplode : BulletModifier
{
    [Header("Behavior")]
    public float radius;
    public float force;

    public GameObject ExplosionEffect;

    public override bool HasExecutable => true;
    public override void Execute(GameObject bullet)
    {
        Vector3 pos = bullet.transform.position;
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        foreach (Collider col in Physics.OverlapSphere(pos, radius))
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && rb != bulletRb) rb.AddExplosionForce(force, pos, radius);

            // later apply damage correctly, dunno how yet
        }  

        Instantiate(ExplosionEffect, bullet.transform.position, Quaternion.identity);
    }
}