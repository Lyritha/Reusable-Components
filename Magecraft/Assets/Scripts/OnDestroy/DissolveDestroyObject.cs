using UnityEngine;
using System.Collections;

public class DissolveDestroyObject : DestroyEffect
{
    [SerializeField]
    private float dissolveTime = 1f;

    public override void TriggerDestroy()
    {
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        Vector3 startScale = transform.localScale;
        float timer = 0f;

        while (timer < dissolveTime)
        {
            timer += Time.deltaTime;
            float t = timer / dissolveTime;

            // Smooth shrink (ease-out)
            float scale = Mathf.Lerp(1f, 0f, t);
            transform.localScale = startScale * scale;

            yield return null;
        }

        Destroy(gameObject);
    }
}
