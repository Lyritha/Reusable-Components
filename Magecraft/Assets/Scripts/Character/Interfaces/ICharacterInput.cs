using System;
using UnityEngine;

public interface ICharacterInput
{
    event Action<Vector2> MoveEvent;

    event Action<Vector2> LookEvent;

    event Action SwitchLookEvent;

    event Action<bool> SprintEvent;

    event Action JumpEvent;
    event Action<bool> AttackEvent;

    event Action<int> NumberSelectEvent;
}
