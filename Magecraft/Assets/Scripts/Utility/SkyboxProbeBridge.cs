using UnityEngine;

public class SkyboxProbeBridge : MonoBehaviour
{
    public ReflectionProbe probe;
    public RenderTexture skyboxCubemap;

    void LateUpdate()
    {
        if (probe.texture != null)
        {
            Graphics.CopyTexture(probe.texture, skyboxCubemap);
        }
    }
}
