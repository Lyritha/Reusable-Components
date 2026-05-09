using UnityEngine;

[ExecuteAlways]
public class SkyboxCubemapRenderer : MonoBehaviour
{
    public Camera skyboxCam;
    public RenderTexture cubemapRT; // must be Cubemap dimension

    void LateUpdate()
    {
        skyboxCam.transform.position = transform.position;
        skyboxCam.RenderToCubemap(cubemapRT);
    }
}
