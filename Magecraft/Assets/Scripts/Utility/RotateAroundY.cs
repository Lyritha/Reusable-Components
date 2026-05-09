using UnityEngine;

public class RotateAroundY : MonoBehaviour
{
    [SerializeField] private float speed = 30f;

    void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.Self);
    }
}
