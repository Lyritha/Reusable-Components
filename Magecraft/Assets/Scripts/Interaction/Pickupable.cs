using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Pickupable : MonoBehaviour
{
    [SerializeField]
    private PlayerInput input;

    [SerializeField]
    private ScriptableItem item;
    [SerializeField]
    private int amount;

    [SerializeField]
    private Image icon;
    [SerializeField]
    private TMP_Text title;

    public UnityEvent OnPickup;
    private bool hasBeenPickedUp = false;

    private void Awake()
    {
        if (item != null)
        {
            icon.sprite = item.Icon;
            title.text = $"Collect {amount} {item.Name}";
        }
    }

    private void OnEnable()
    {
        input.OnPerformed += OnInteract;
    }

    private void OnDisable()
    {
        input.OnPerformed -= OnInteract;
    }



    private void OnInteract()
    {
        if (hasBeenPickedUp) return;

        if (Inventory.Instance != null)
        {
            hasBeenPickedUp = true;
            Inventory.Instance.Items.AddItems(item, amount, item.type);
            OnPickup?.Invoke();
        }
    }

}
