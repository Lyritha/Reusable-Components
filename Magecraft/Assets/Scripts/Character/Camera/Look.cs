using UnityEngine;

public class Look : InputListener
{
    [SerializeField] protected float yawSensitivity = 0.5f;
    [SerializeField] protected float pitchSensitivity = 0.5f;

    protected float yaw;
    protected float pitch;

    protected Vector2 lookInput;

    protected virtual void Awake()
    {
        // handles component exclucivity, copies base class info
        foreach (Look look in GetComponents<Look>())
        {
            if (look != this)
            {
                CopyBaseFieldsFrom(look);
                Destroy(look);
            }
        }

        yaw = transform.eulerAngles.y;

        AddSubscription(e => e.OnLookDelta += OnLook, e => e.OnLookDelta -= OnLook);
    }

    public void OnLook(Vector2 dir) => lookInput = dir;

    protected void CopyBaseFieldsFrom(Look other)
    {
        yawSensitivity = other.yawSensitivity;
        pitchSensitivity = other.pitchSensitivity;
        yaw = other.yaw;
        pitch = other.pitch;
        lookInput = other.lookInput;
    }
}
