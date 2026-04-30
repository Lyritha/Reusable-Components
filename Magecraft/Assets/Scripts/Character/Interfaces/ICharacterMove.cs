using UnityEngine;

public interface ICharacterMove
{
    void Move(Vector2 dir);

    Vector2 MoveSpeed { get; set; }
}
