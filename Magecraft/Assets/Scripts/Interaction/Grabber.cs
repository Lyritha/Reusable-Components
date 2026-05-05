using UnityEngine;

public class Grabber : MonoBehaviour
{
    [SerializeField]
    private float grabRange = 3.0f;
    [SerializeField]
    private LayerMask grabMask = ~0;

    private ICharacterInput input;
    private ICharacterInput lastInput;

    private IGrabbable current;
    private GameObject currentObj;

    private Vector3 lastValidHit = Vector3.zero;


    private void OnAttack(bool pressed)
    {
        if (pressed) TryGrab();
        else TryRelease();
    }

    private void Update()
    {
        input = GetComponentInParent<ICharacterInput>();
        if (input != lastInput) SwapInput(lastInput, input);

        if (current != null) UpdateGrabStay();
    }

    private void TryGrab()
    {
        if (!MouseRaycast.TryGetWorldMouseHit(out RaycastHit hit, grabMask, grabRange)) return;
        if (!hit.collider.TryGetComponent(out IGrabbable grabbable)) return;

        current = grabbable;
        currentObj = hit.collider.gameObject;

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
        currentObj = null;
    }

    private void SwapInput(ICharacterInput oldInput, ICharacterInput newInput)
    {
        if (oldInput != null) oldInput.AttackEvent -= OnAttack;
        if (newInput != null) newInput.AttackEvent += OnAttack;
        lastInput = input;
    }

    private void OnDisable()
    {
        if (lastInput != null) lastInput.AttackEvent -= OnAttack;
    }
}
