using UnityEngine;

public class SimpleDestroyObject : DestroyEffect
{
    public override void TriggerDestroy() => Destroy(gameObject);
}
