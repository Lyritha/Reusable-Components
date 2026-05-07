using UnityEngine;

[CreateAssetMenu(menuName = "Bullet/StatModifier")]
public class BulletModifier : ScriptableObject
{
    [Header("UI")]
    public string BehaviorName;
    [TextArea] public string BehaviorInfo;

    [Header("Functionality")]
    public BulletModifierType type;
    public BulletStats statModifier;

    public virtual bool HasExecutable => false;
    public virtual void Execute(GameObject bullet) { }
}


public enum BulletModifierType
{
    None,
    OnShot,
    OnMoving,
    OnImpact, 
    OnRangeReached
}