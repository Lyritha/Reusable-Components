using System;
using UnityEngine;

public class BridgeController : EntityController
{
    [Header("Parent Connection")]
    [SerializeField, Tooltip("The ID of the parent controller to connect to, or -1 if you don't want to connect to a parent by default.")]
    private int parentID = -1;

    private EntityController parent;

    // keep the subscription so we can unsubscribe later
    private Action _parentDestroyedHandler;

    private void OnValidate()
    {
        makeInstance = false;
    }

    protected override void Awake()
    {
        base.Awake();

        if (parentID != -1) ConnectToController(parentID);
    }

    public void ConnectToController(int controllerId)
    {
        if (!TryGet(controllerId, out EntityController newController)) return;
        if (newController == parent) return;

        DisconnectFromController();

        parent = newController;

        // share the LayerItem instances intentionally
        Move = parent.Move;
        LookDelta = parent.LookDelta;

        Sprint = parent.Sprint;
        Jump = parent.Jump;

        PrimaryMouse = parent.PrimaryMouse;
        SecondaryMouse = parent.SecondaryMouse;
        Interact = parent.Interact;

        Tab = parent.Tab;
        NumberSelected = parent.NumberSelected;
        Inventory = parent.Inventory;

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
        Move = new LayerItem<Vector2>();
        LookDelta = new LayerItem<Vector2>();

        Sprint = new LayerItem<bool>();
        Jump = new LayerItem();

        PrimaryMouse = new LayerItem<bool> ();
        SecondaryMouse = new LayerItem<bool> ();
        Interact = new LayerItem ();

        Tab = new LayerItem();
        Inventory = new LayerItem();

        NumberSelected = new LayerItem<int>();

        parent = null;
    }

    protected override void OnDestroy()
    {
        DisconnectFromController();
        base.OnDestroy();
    }
}
