using UnityEngine;
using UnityEngine.Events;

public class InteractableGameobject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private bool interactOnce = false;

    public UnityEvent OnInteracted = new();

    private bool hasBeenInteractedWith = false;

    public void Interact()
    {
        if (interactOnce && hasBeenInteractedWith) return;

        hasBeenInteractedWith = true;
        OnInteracted?.Invoke();
    }
}
