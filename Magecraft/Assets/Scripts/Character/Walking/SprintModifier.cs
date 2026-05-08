using UnityEngine;

public class SprintModifier : InputListener
{
    [SerializeField]
    private Movement4 sprintMult = Movement4.One;
    private Movement4 originalMoveSpeed;

    private WalkController walkController;

    protected void Awake()
    {
        AddSubscription(
             ec => ec.OnSprint += OnSprint,
             ec => ec.OnSprint -= OnSprint
         );
    }

    public void OnSprint(bool isSprinting)
    {
        // grab character move, modify move speed, and set it back to the character move
        if (walkController == null && !TryGetComponent(out walkController)) return;

        if (isSprinting) originalMoveSpeed = walkController.MaxSpeed;
        walkController.MaxSpeed = isSprinting ? originalMoveSpeed * sprintMult : originalMoveSpeed;
    }
}
