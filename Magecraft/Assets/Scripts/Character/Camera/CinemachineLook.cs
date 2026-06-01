using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CinemachineLook : Look
{
    [Header("Body Follow Settings"), SerializeField]
    protected Vector2 pitchLimits = new(-60f, 40f);

    protected CinemachinePanTilt panTilt;
    protected Rigidbody rb;

    public virtual void Initialize(CinemachinePanTilt panTilt)
    {
        rb = GetComponent<Rigidbody>();
        this.panTilt = panTilt;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void Update()
    {
        base.Update();

        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);
        if (panTilt != null) panTilt.TiltAxis.Value = pitch;
    }

    private void FixedUpdate() => rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));

    public CinemachinePanTilt PanTilt => panTilt;
}
