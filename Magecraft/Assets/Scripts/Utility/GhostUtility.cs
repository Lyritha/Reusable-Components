using UnityEngine;

public static class GhostUtility
{
    public static GameObject CreateGhost(GameObject source, bool setActive = true)
        => CreateGhostInternal(source, null, setActive);
    public static GameObject CreateGhost(GameObject source, Transform parent, bool setActive = true)
        => CreateGhostInternal(source, parent, setActive);

    public static GameObject CreateGhost(Object source, bool setActive = true)
        => CreateGhostInternal(ExtractGameObject(source), null, setActive);
    public static GameObject CreateGhost(Object source, Transform parent, bool setActive = true)
        => CreateGhostInternal(ExtractGameObject(source), parent, setActive);


    private static GameObject CreateGhostInternal(GameObject source, Transform parent, bool setActive)
    {
        GameObject ghost = Object.Instantiate(source, parent);
        StripComponents(ghost);

        foreach (Renderer rend in ghost.GetComponentsInChildren<Renderer>())
        {
            // Create a NEW material instance so the original is untouched
            Material ghostMat = new Material(rend.sharedMaterial);

            // Switch to transparent mode (URP/Lit)
            ghostMat.SetFloat("_Surface", 1); // Transparent
            ghostMat.SetFloat("_Blend", 0);   // Alpha blend
            ghostMat.SetFloat("_ZWrite", 0);  // Disable depth writes
            ghostMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            // Optional: ensure correct blending
            ghostMat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            ghostMat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            // Apply ghost color
            Color c = ghostMat.GetColor("_BaseColor");
            c.a = 0.25f;
            ghostMat.SetColor("_BaseColor", c);

            // Assign the new material instance
            rend.material = ghostMat;
        }


        ghost.SetActive(setActive);
        return ghost;
    }

    private static GameObject ExtractGameObject(Object source)
    {
        if (source is GameObject go) return go;
        if (source is Component comp) return comp.gameObject;

        throw new System.ArgumentException(
            "GhostUtility.CreateGhost: Source must be a GameObject or Component.",
            nameof(source)
        );
    }

    private static void StripComponents(GameObject ghost)
    {
        foreach (MonoBehaviour comp in ghost.GetComponentsInChildren<MonoBehaviour>()) Object.Destroy(comp);
        foreach (Collider col in ghost.GetComponentsInChildren<Collider>()) Object.Destroy(col);
        foreach (Rigidbody rb in ghost.GetComponentsInChildren<Rigidbody>()) Object.Destroy(rb);
    }
}
