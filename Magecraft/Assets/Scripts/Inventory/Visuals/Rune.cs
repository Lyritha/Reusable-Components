using UnityEngine;

public class Rune : MonoBehaviour, IGrabbable
{
    [SerializeField]
    private float tableHeightOffset = .05f;

    public bool IsGrabbed { get; private set; }

    private Vector3 grabOffset = Vector3.zero;

    public void GrabStart(Vector3 grabberPos)
    {
        IsGrabbed = true;

        grabOffset = transform.position - grabberPos;
    }

    public void GrabStay(Vector3 grabberPos)
    {
        if (!IsGrabbed) return;

        transform.position = GetGrabbedPosition(grabberPos + grabOffset);
    }

    public void GrabStop(Vector3 grabberPos)
    {
        IsGrabbed = false;
    }

    private Vector3 GetGrabbedPosition(Vector3 rayPos)
    {
        Ray ray = new(rayPos, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = hit.point;
            pos += Vector3.up * tableHeightOffset;
            return pos;
        }

        return transform.position;
    }
}
