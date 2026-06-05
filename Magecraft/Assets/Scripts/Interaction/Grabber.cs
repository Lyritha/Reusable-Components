using UnityEngine;

public class Grabber : InputListener
{
    [SerializeField]
    private float grabRange = 3.0f;
    [SerializeField]
    private LayerMask grabMask = ~0;

    private IGrabbable current;

    private Vector3 lastValidHit = Vector3.zero;


    private void Awake()
    {
        AddSubscription(e => e.PrimaryMouse.OnEvent += OnAttack, e => e.PrimaryMouse.OnEvent -= OnAttack);
    }

    private void OnAttack(bool pressed)
    {
        if (pressed) TryGrab();
        else TryRelease();
    }

    protected override void Update()
    {
        base.Update();

        if (current != null) UpdateGrabStay();
    }

    private void TryGrab()
    {
        if (!MouseRaycast.TryGetWorldMouseHit(out RaycastHit hit, grabMask, grabRange)) return;
        if (!hit.collider.TryGetComponent(out IGrabbable grabbable)) return;

        current = grabbable;

        lastValidHit = hit.point;
        current.GrabStart(lastValidHit);
    }

    private void UpdateGrabStay()
    {
        if (MouseRaycast.TryGetWorldMouseHit(out RaycastHit hit, grabMask, grabRange)) current.GrabStay(hit.point);
    }

    private void TryRelease()
    {
        if (current == null) return;

        Vector3 releasePos = lastValidHit;
        if (MouseRaycast.TryGetWorldMouseHit(out RaycastHit hit, grabMask, grabRange)) lastValidHit = releasePos = hit.point;

        current.GrabStop(releasePos);

        current = null;
    }
}
