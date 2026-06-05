using System;
using UnityEngine;

public class Interactor : InputListener
{
    [SerializeField]
    private float maxDistance = 4;

    private IInteractable target;
    private bool canInteract = false;

    private void Awake()
    {
        ContinuesRaycast.EnsureExistence();
        ContinuesRaycast.OnRayEntered += OnRayEntered;
        ContinuesRaycast.OnRayExited += OnRayExited;

        AddSubscription(e => e.Interact.OnEvent += Interact, e => e.Interact.OnEvent -= Interact);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        ContinuesRaycast.OnRayEntered -= OnRayEntered;
        ContinuesRaycast.OnRayExited -= OnRayExited;
    }

    private void Interact()
    {
        if (!canInteract) return;
        target?.Interact();
    }

    private void OnRayEntered(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent(out IInteractable interactable))
        {
            canInteract = hit.distance <= maxDistance;
            target = interactable;
        }
        else OnRayExited();
    }

    private void OnRayExited()
    {
        canInteract = false;
        target = null;
    }
}
