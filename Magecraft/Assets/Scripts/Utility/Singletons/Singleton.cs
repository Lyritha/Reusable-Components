using UnityEngine;

[Fold]
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField, Tooltip("If true, this instance will persist across scene loads")]
    protected bool persistentInstance = false;
    [SerializeField, Tooltip("If true, destroy whole gameobject if there already is an instance, instead of just instance")]
    protected bool removeWholeObject = true;

    public static T Instance { get; protected set; }


    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (removeWholeObject) Destroy(gameObject);
            else Destroy(this);

            return;
        }

        Instance = this as T;

        if (persistentInstance) DontDestroyOnLoad(gameObject);
    }
}
