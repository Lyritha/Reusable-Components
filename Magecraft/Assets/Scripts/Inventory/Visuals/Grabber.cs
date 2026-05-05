using UnityEngine;

public class Grabber : MonoBehaviour
{
    [SerializeField]
    private float grabRange = 3.0f;

    [SerializeField]
    private LayerMask grabbableMask;

    private IGrabbable current;
    private Transform cam;

    Vector3 lastValidHitPos = Vector3.zero;

    private void Awake()
    {
        cam = Camera.main.transform;
    }


    private void TryGrab()
    {
        Ray ray = new(cam.position, cam.forward);

        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, grabRange, grabbableMask);
        if (!hitSomething) return;

        if (!hit.collider.TryGetComponent(out IGrabbable grabbable)) return;

        current = grabbable;
        current.GrabStart(hit.point);
    }

    private void TryStay()
    {
        if (current == null) return;

        // Always use the camera ray to determine the grabber position
        Ray ray = new(cam.position, cam.forward);
        Vector3 grabberPos = ray.GetPoint(1.0f); // 1 meter in front of camera

        current.GrabStay(grabberPos);
    }

    private void TryRelease()
    {
        if (current == null)
        {
            return;
        }

        Ray ray = new(cam.position, cam.forward);
        Vector3 grabberPos = ray.GetPoint(1.0f);

        current.GrabStop(grabberPos);
        current = null;
    }
}
