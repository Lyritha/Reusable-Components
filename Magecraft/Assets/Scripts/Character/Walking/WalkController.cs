using UnityEngine;

public class WalkController : InputListener
{
    [SerializeField]
    protected Movement4 maxSpeed = Movement4.One;
    [SerializeField]
    protected float acceleration = 5;

    protected Vector2 dir = Vector2.zero;
    protected Vector2 localVelocity = Vector2.zero;
    private Rigidbody rb;

    [SerializeField, ShowOnly]
    protected Movement4 currentMaxSpeed;


    protected  void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentMaxSpeed = maxSpeed;

        AddSubscription(
             ec => ec.Move.OnEvent += OnMove,
             ec => ec.Move.OnEvent -= OnMove
         );
    }

    private void OnMove(Vector2 vector)
    {
        dir = vector;
    }

    protected virtual void FixedUpdate()
    {
        localVelocity = GetLocalVelocity(localVelocity, dir, currentMaxSpeed, acceleration);

        if (rb != null)
        {
            Vector3 worldVelocity = transform.forward * localVelocity.y + transform.right * localVelocity.x;
            worldVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = worldVelocity;
        }
    }

    protected Vector2 GetLocalVelocity(Vector2 currentVel, Vector2 input, Movement4 speedMod, float accel)
    {

        Vector2 targetVel = Vector2.zero;
        if (input.sqrMagnitude > 0.1f)
        {
            targetVel.x = input.x > 0 ? input.x * speedMod.right : input.x * speedMod.left;
            targetVel.y = input.y > 0 ? input.y * speedMod.forward : input.y * speedMod.backward;
        }

        return Vector2.MoveTowards(currentVel, targetVel, accel * Time.fixedDeltaTime);
    }

    public Movement4 MaxSpeed { get { return maxSpeed; }}
    public Movement4 CurrentMaxSpeed { get { return currentMaxSpeed; } set { currentMaxSpeed = value; } }
}
