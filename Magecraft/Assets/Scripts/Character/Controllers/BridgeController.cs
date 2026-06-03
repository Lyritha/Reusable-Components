using System;
using UnityEngine;

public class BridgeController : EntityController
{
    private EntityController parent;

    // keep the subscription so we can unsubscribe later
    private Action _parentDestroyedHandler;

    public void ConnectToController(int controllerId)
    {
        if (!TryGet(controllerId, out EntityController newController)) return;
        if (newController == parent) return;

        DisconnectFromController();

        parent = newController;

        // share the LayerItem instances intentionally
        OnMove = parent.OnMove;
        OnLookDelta = parent.OnLookDelta;

        OnSprint = parent.OnSprint;
        OnJump = parent.OnJump;

        OnPrimaryMouse = parent.OnPrimaryMouse;
        OnSecondaryMouse = parent.OnSecondaryMouse;
        OnInteract = parent.OnInteract;

        OnTab = parent.OnTab;
        OnNumberSelected = parent.OnNumberSelected;
        OnInventory = parent.OnInventory;

        // subscribe to parent's destroy so we can break the shared references
        _parentDestroyedHandler = () => DisconnectFromController();
        parent.OnControllerDestroyed += _parentDestroyedHandler;
    }

    public void DisconnectFromController()
    {
        // if we were subscribed to parent's destroy, remove it
        if (parent != null && _parentDestroyedHandler != null)
        {
            parent.OnControllerDestroyed -= _parentDestroyedHandler;
            _parentDestroyedHandler = null;
        }

        // break shared references by creating fresh LayerItems
        OnMove = new LayerItem<Vector2>();
        OnLookDelta = new LayerItem<Vector2>();

        OnSprint = new LayerItem<bool>();
        OnJump = new LayerItem();

        OnPrimaryMouse = new LayerItem<bool> ();
        OnSecondaryMouse = new LayerItem<bool> ();
        OnInteract = new LayerItem ();

        OnTab = new LayerItem();
        OnInventory = new LayerItem();

        OnNumberSelected = new LayerItem<int>();

        parent = null;
    }

    protected override void OnDestroy()
    {
        DisconnectFromController();
        base.OnDestroy();
    }
}
