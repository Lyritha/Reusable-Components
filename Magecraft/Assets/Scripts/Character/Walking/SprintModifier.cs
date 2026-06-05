using UnityEngine;

public class SprintModifier : InputListener
{
    [SerializeField]
    private Movement4 sprintMult = Movement4.One;
    private Movement4 originalMoveSpeed;

    private WalkController walkController;

    protected void Awake()
    {
        if (TryGetComponent(out walkController)) originalMoveSpeed = walkController.MaxSpeed;

        AddSubscription(
             ec => ec.Sprint.OnEvent += OnSprint,
             ec => ec.Sprint.OnEvent -= OnSprint
         );
    }

    public void OnSprint(bool isSprinting)
    {
        // grab character move, modify move speed, and set it back to the character move
        if (walkController == null && !TryGetComponent(out walkController)) return;

        if (isSprinting) originalMoveSpeed = walkController.MaxSpeed;
        walkController.CurrentMaxSpeed = isSprinting ? originalMoveSpeed * sprintMult : originalMoveSpeed;
    }
}
