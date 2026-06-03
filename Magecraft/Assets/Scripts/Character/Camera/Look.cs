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

        yaw = WrapAngle(transform.eulerAngles.y);

        AddSubscription(e => e.OnLookDelta.OnEvent += OnLook, e => e.OnLookDelta.OnEvent -= OnLook);
    }

    protected override void Update()
    {
        base.Update();

        // wrap by default, clamp in derived classes if needed
        yaw += lookInput.x * yawSensitivity;
        yaw = WrapAngle(yaw);

        pitch -= lookInput.y * pitchSensitivity;
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

    protected static float WrapAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f) angle -= 360f;
        else if (angle < -180f) angle += 360f;
        return angle;
    }
}
