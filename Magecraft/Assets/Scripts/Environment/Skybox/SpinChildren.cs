using System.Collections.Generic;
using UnityEngine;

public class SpinChildren : MonoBehaviour
{
    private List<(Transform, float)> children;

    private void Awake()
    {
        children = new List<(Transform, float)>();

        foreach (Transform t in transform)
        {
            (Transform, float) pair = new();

            MaterialPropertyBlock block = new();
            pair.Item2 = Random.value;
            block.SetFloat("_Seed", Random.value);

            Renderer render = t.GetComponent<Renderer>();
            render.SetPropertyBlock(block);

            pair.Item1 = t;
            children.Add(pair);
        }
        
    }

    private void Update()
    {
        foreach ((Transform, float) pair in children)
        {
            pair.Item1.Rotate(0, pair.Item2 * Time.deltaTime, 0);
        }
    }
}
