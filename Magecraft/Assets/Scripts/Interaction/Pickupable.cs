using System;
using UnityEngine;
using UnityEngine.Events;

public class Pickupable : InputListener
{
    [SerializeField]
    private ScriptableItem item;
    [SerializeField]
    private int amount;

    public UnityEvent OnPickup;
    private bool hasBeenPickedUp = false;

    private void Awake()
    {
        AddSubscription(
            entity => entity.Interact.OnEvent += OnInteract,
            entity => entity.Interact.OnEvent -= OnInteract
        );
    }



    private void OnInteract()
    {
        if (hasBeenPickedUp) return;

        uint id = entity.InstanceId;

        Debug.Log($"finding inventory with id: {id}");
        if (Inventory.TryGet(id, out Inventory inventory))
        {
            Debug.Log("found inventory");

            hasBeenPickedUp = true;
            inventory.InventoryPouch.AddItems(item, amount);
            OnPickup?.Invoke();
        }
    }

}
