using System;

[Serializable]
public struct Movement4
{
    public float forward;
    public float backward;
    public float left;
    public float right;

    public static Movement4 operator *(Movement4 a, Movement4 b)
    {
        return new Movement4
        {
            forward = a.forward * b.forward,
            backward = a.backward * b.backward,
            left = a.left * b.left,
            right = a.right * b.right
        };
    }

    public static Movement4 operator *(Movement4 a, float f)
    {
        return new Movement4
        {
            forward = a.forward * f,
            backward = a.backward * f,
            left = a.left * f,
            right = a.right * f
        };
    }

    public static readonly Movement4 One = new()
    {
        forward = 1,
        backward = 1,
        left = 1,
        right = 1
    };

}
