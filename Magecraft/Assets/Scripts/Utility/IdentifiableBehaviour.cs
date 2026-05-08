using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IdentifiableBehaviour<T> : MonoBehaviour where T : IdentifiableBehaviour<T>
{
    private static readonly Dictionary<uint, T> registry = new();
    private static uint nextId = 0;

    // handle forced and non-forced id's safely
    private static int pendingForcedCount = 0;
    private static bool forcedPhaseComplete = false;

    [SerializeField, Tooltip("If not -1, force a specific ID for this (or the parent class) type")]
    private int forcedId = -1;
    [SerializeField, ShowOnly]
    private int Id = 0;

    public uint InstanceId { get; private set; } = uint.MaxValue;

    protected virtual void Awake()
    {
        if (InstanceId != uint.MaxValue) return;

        if (forcedId >= 0)
        {
            // add to pending, and ensure if a new one is added after completion is already true
            pendingForcedCount++;
            forcedPhaseComplete = false;

            uint forcedUnsignedId = (uint)forcedId;
            while (registry.TryGetValue(forcedUnsignedId, out T existing) && existing != (T)this)
            {
                registry.Remove(forcedUnsignedId);

                // finds next available id for the swapped instance
                while (registry.ContainsKey(nextId)) nextId++;

                existing.InstanceId = nextId;
                registry.Add(nextId, existing);
            }

            registry[forcedUnsignedId] = (T)this;
            InstanceId = forcedUnsignedId;
            Id = (int)forcedUnsignedId;

            // remove from pending, if no are pending set finished to true
            pendingForcedCount--;
            forcedPhaseComplete = pendingForcedCount == 0;

            return;
        }

        // if non forced, create wait delay
        StartCoroutine(AssignDefaultWhenReady());
    }

    private IEnumerator AssignDefaultWhenReady()
    {
        yield return new WaitUntil(() => forcedPhaseComplete);

        // finds next available id for this instance
        while (registry.ContainsKey(nextId)) nextId++;

        // Assign this instance
        registry[nextId] = (T)this;
        InstanceId = nextId;
        Id = (int)nextId;
    }

    protected virtual void OnDestroy() => registry.Remove(InstanceId);
    public static bool TryGet(int id, out T instance)
    {
        instance = null;
        return id >= 0 && registry.TryGetValue((uint)id, out instance);
    }
    public static bool TryGet(uint id, out T instance) => registry.TryGetValue(id, out instance);
    public static IReadOnlyDictionary<uint, T> Registry => registry;
}
