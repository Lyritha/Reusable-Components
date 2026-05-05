using UnityEngine;

public interface IGrabbable
{
    bool IsGrabbed { get; }

    void GrabStart(Vector3 grabberPos);
    void GrabStay(Vector3 grabberPos);
    void GrabStop(Vector3 grabberPos);
}