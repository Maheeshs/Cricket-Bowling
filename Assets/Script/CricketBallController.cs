using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CricketBallController : MonoBehaviour
{
    public enum BallType { Swing, Spin }
    public enum SwingType { Inswing, Outswing }
    public enum SpinType { OffSpin, LegSpin }

    public BallType ballType;
    public SwingType swingType;
    public SpinType spinType;

    public float swingStrength = 6f;
    public float spinStrength = 6f;

    Rigidbody rb;

    Vector3 startPos;
    Vector3 targetPos;
    Vector3 controlPoint;

    float flightTime = 1.2f;
    float timer;

    bool launched;
    bool bounced;

    Vector3 lastPosition;
    Vector3 tangentDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, 10f);
    }

    public void LaunchBall(Vector3 velocity, Vector3 start, Vector3 target)
    {
        launched = true;

        startPos = start;
        targetPos = target;

        timer = 0f;

        rb.isKinematic = true;

        // SWING CONTROL POINT
        Vector3 mid = (start + target) * 0.5f;

        if (ballType == BallType.Swing)
        {
            Vector3 side =
                (swingType == SwingType.Outswing)
                ? Vector3.right
                : Vector3.left;

            controlPoint = mid + side * swingStrength * 2.5f;
        }
        else
        {
            // Spin bowls mostly straight
            controlPoint = mid;
        }

        lastPosition = start;
    }

    void Update()
    {
        if (!launched || bounced)
            return;

        timer += Time.deltaTime;

        float t = timer / flightTime;
        t = Mathf.Clamp01(t);

        // QUADRATIC BEZIER
        Vector3 pos =
            Mathf.Pow(1 - t, 2) * startPos +
            2 * (1 - t) * t * controlPoint +
            Mathf.Pow(t, 2) * targetPos;

        // Height arc
        pos.y += Mathf.Sin(t * Mathf.PI) * 2f;

        transform.position = pos;

        // Tangent direction
        tangentDirection = (pos - lastPosition).normalized;

        if (tangentDirection != Vector3.zero)
            transform.forward = tangentDirection;

        lastPosition = pos;

        // REACHED MARKER
        if (t >= 1f)
        {
            BounceBall();
        }
    }

    void BounceBall()
    {
        bounced = true;

        rb.isKinematic = false;

        // Lift slightly above ground before physics starts
        transform.position += Vector3.up * 0.15f;

        // Tangent forward speed
        Vector3 bounceVelocity = tangentDirection * 18f;

        // REAL visible bounce
        bounceVelocity.y = 6f;

        // Massive spin turn
        if (ballType == BallType.Spin)
        {
            Vector3 side =
                (spinType == SpinType.OffSpin)
                ? Vector3.right
                : Vector3.left;

            bounceVelocity += side * spinStrength * 4f;
        }

        rb.velocity = bounceVelocity;
    }
}