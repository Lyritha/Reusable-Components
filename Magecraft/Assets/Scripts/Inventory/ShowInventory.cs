using UnityEngine;

public class ShowInventory : InputListener
{
    [SerializeField]
    private PotionDisplay potionDisplay;

    private bool inventoryOpen = false;

    private void Awake()
    {
        AddSubscription(e => e.OnInventory.OnEvent += OnInventory, e => e.OnInventory.OnEvent -= OnInventory);
    }

    private void OnInventory()
    {
        inventoryOpen = !inventoryOpen;

        if (inventoryOpen)
        {
            potionDisplay.gameObject.SetActive(true);
            entity.ActiveLayer = Layer.UI;
            potionDisplay.ShowPotions(20);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            potionDisplay.HidePotions();
            potionDisplay.OnFinishedAnimating += AnimCompleted;
        }
    }

    private void AnimCompleted()
    {
        potionDisplay.gameObject.SetActive(false);
        entity.ActiveLayer = Layer.Movement;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        potionDisplay.OnFinishedAnimating -= AnimCompleted;
    }
}
