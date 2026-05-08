using System;
using UnityEngine;

public class EntityController : IdentifiableBehaviour<EntityController>
{
// enclosed in pragma to avoid "unused" warnings for entities that don't use certain events
#pragma warning disable CS0067

    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLookDelta;

    public event Action<bool> OnSprint;
    public event Action OnJump;

    public event Action<bool> OnPrimaryMouse;
    public event Action<bool> OnSecondaryMouse;

    public event Action OnTab;
    public event Action<int> OnNumberSelected;


    protected void RaiseMove(Vector2 v) => OnMove?.Invoke(v);
    protected void RaiseLookDelta(Vector2 v) => OnLookDelta?.Invoke(v);
    protected void RaiseSprint(bool v) => OnSprint?.Invoke(v);
    protected void RaiseJump() => OnJump?.Invoke();
    protected void RaisePrimaryMouse(bool v) => OnPrimaryMouse?.Invoke(v);
    protected void RaiseSecondaryMouse(bool v) => OnSecondaryMouse?.Invoke(v);
    protected void RaiseTab() => OnTab?.Invoke();
    protected void RaiseNumberSelected(int v) => OnNumberSelected?.Invoke(v);
#pragma warning restore CS0067
}
