using UnityEngine;

public class CharacterSprint : MonoBehaviour, ICharacterSprint
{
    [SerializeField]
    private float sprintMult = 2;

    private Vector2 originalMoveSpeed = Vector2.zero;

    public void OnSprint(bool isSprinting)
    {
        // grab character move, modify move speed, and set it back to the character move
        ICharacterMove characterMove = GetComponent<ICharacterMove>();
        if (characterMove == null) return;

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
