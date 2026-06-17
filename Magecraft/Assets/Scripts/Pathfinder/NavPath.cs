using UnityEngine;

public class NavPath
{
    public Vector3[] Points;

    public int Length => Points?.Length ?? 0;

    public Vector3 this[int i] => Points[i];

    public static implicit operator NavPath(Vector3[] points) => new() { Points = points };
}
