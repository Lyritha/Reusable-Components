using System;
using UnityEngine;

public class BridgeController : EntityController
{
    [Header("Parent Connection")]
    [SerializeField, Tooltip("The ID of the parent controller to connect to, or -1 if you don't want to connect to a parent by default.")]
    private int parentID = -1;

    private EntityController parent;

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
        parentID = controllerId;
        InstanceId = (uint)controllerId;

        // forward events instead of replacing LayerItems
        Move.SetSource(parent.Move);
        LookDelta.SetSource(parent.LookDelta);

        Sprint.SetSource(parent.Sprint);
        Jump.SetSource(parent.Jump);

        PrimaryMouse.SetSource(parent.PrimaryMouse);
        SecondaryMouse.SetSource(parent.SecondaryMouse);
        Interact.SetSource(parent.Interact);

        Tab.SetSource(parent.Tab);
        NumberSelected.SetSource(parent.NumberSelected);
        Inventory.SetSource(parent.Inventory);

        // auto-disconnect if parent is destroyed
        _parentDestroyedHandler = () => DisconnectFromController();
        parent.OnControllerDestroyed += _parentDestroyedHandler;
    }

    public void DisconnectFromController()
    {
        parentID = -1;

        if (parent != null && _parentDestroyedHandler != null)
        {
            parent.OnControllerDestroyed -= _parentDestroyedHandler;
            _parentDestroyedHandler = null;
        }

        // stop forwarding, but keep LayerItem instances alive
        Move.SetSource(null);
        LookDelta.SetSource(null);

        Sprint.SetSource(null);
        Jump.SetSource(null);

        PrimaryMouse.SetSource(null);
        SecondaryMouse.SetSource(null);
        Interact.SetSource(null);

        Tab.SetSource(null);
        NumberSelected.SetSource(null);
        Inventory.SetSource(null);

        parent = null;
    }

    protected override void OnDestroy()
    {
        DisconnectFromController();
        base.OnDestroy();
    }
}

