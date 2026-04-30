using UnityEngine;

public class SprintModifier : MonoBehaviour, ICharacterSprint
{
    [SerializeField]
    private float sprintMult = 2;

    private Vector2 originalMoveSpeed = Vector2.zero;

    public void OnSprint(bool isSprinting)
    {
        // grab character move, modify move speed, and set it back to the character move
        if (!TryGetComponent<ICharacterMove>(out var characterMove)) return;

        if (isSprinting) {
            originalMoveSpeed = characterMove.MoveSpeed;
            Vector2 newSpeed = originalMoveSpeed * sprintMult;
            characterMove.MoveSpeed = newSpeed;
        }
        else 
        {
            characterMove.MoveSpeed = originalMoveSpeed;
        }
    }
}
