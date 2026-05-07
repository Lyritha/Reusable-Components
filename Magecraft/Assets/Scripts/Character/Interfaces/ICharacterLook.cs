using UnityEngine;

public interface ICharacterLook
{
    void OnLook(Vector2 dir);
    void OnWantToLook(bool enableLook);
}
