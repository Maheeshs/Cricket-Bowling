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
    public float spinStrength = 4f;

    public LayerMask groundLayer;

    Rigidbody rb;

    [HideInInspector] public Vector3 trueDirection;
    [HideInInspector] public float preBounceSpeed;

    bool hasBowled;
    bool hasBounced;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Awake() => rb = GetComponent<Rigidbody>();
    void Start() => Destroy(gameObject, 10f);

    public void LaunchBall(Vector3 velocity, Vector3 start, Vector3 target)
    {
        hasBowled = true;
        preBounceSpeed = velocity.magnitude;
        trueDirection = velocity.normalized;
        rb.velocity = velocity;

        startPos = start;
        targetPos = target;
    }

    void FixedUpdate()
    {
        if (!hasBowled || hasBounced) return;

        if (ballType == BallType.Swing)
        {
            // Inswing = curve left, Outswing = curve right (world axes)
            Vector3 side = (swingType == SwingType.Outswing) ? Vector3.right : Vector3.left;

            float totalDist = Vector3.Distance(startPos, targetPos);
            float traveled = Vector3.Distance(startPos, transform.position);
            float progress = Mathf.Clamp01(traveled / totalDist);

            // Swing force peaks mid‑flight, fades near marker
            float swingForce = Mathf.Sin(progress * Mathf.PI) * swingStrength;

            // Increase multiplier for stronger swing
            rb.AddForce(side * swingForce * 1.2f, ForceMode.Acceleration);

            trueDirection = rb.velocity.normalized;
        }

        else if (ballType == BallType.Spin)
        {
            // Apply torque for seam rotation
            float spinTorque = (spinType == SpinType.OffSpin ? 1f : -1f) * spinStrength * 50f;
            rb.AddTorque(Vector3.up * spinTorque, ForceMode.Force);

            trueDirection = rb.velocity.normalized;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && !hasBounced)
        {
            hasBounced = true;

            Vector3 straight = trueDirection * preBounceSpeed;
            Vector3 vel = straight + Vector3.up * 1.2f;

            if (ballType == BallType.Spin)
            {
                // Off‑spin kicks right, Leg‑spin kicks left
                Vector3 side = (spinType == SpinType.OffSpin) ? Vector3.right : Vector3.left;
                vel += side * spinStrength * (preBounceSpeed * 0.1f);
            }

            rb.velocity = vel;
        }
    }
}
