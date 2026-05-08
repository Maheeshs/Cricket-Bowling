using UnityEngine;
using System.Collections;

public class BowlingManager : MonoBehaviour
{
    public CricketBallController ballPrefab;
    public Transform bowlPoint;
    public BounceMarkerController marker;
    public BowlingMeter bowlingMeter;

    public float nextBallDelay = 2f;

    private CricketBallController currentBall;
    private bool canBowl = true;

    // Current selection
    private CricketBallController.BallType selectedBallType;
    private CricketBallController.SwingType selectedSwingType;
    private CricketBallController.SpinType selectedSpinType;

    void Start()
    {
        // Default: Swing Inswing
        SelectSwingType();
        SelectInswing();
    }

    // =========================
    // Top‑level Ball Type Buttons
    // =========================
    public void SelectSwingType()
    {
        selectedBallType = CricketBallController.BallType.Swing;
        Debug.Log("Ball type set to Swing");
    }

    public void SelectSpinType()
    {
        selectedBallType = CricketBallController.BallType.Spin;
        Debug.Log("Ball type set to Spin");
    }

    // =========================
    // Subtype Buttons
    // =========================
    public void SelectInswing()
    {
        selectedSwingType = CricketBallController.SwingType.Inswing;
        Debug.Log("Selected Swing subtype: Inswing");
    }

    public void SelectOutswing()
    {
        selectedSwingType = CricketBallController.SwingType.Outswing;
        Debug.Log("Selected Swing subtype: Outswing");
    }

    public void SelectOffSpin()
    {
        selectedSpinType = CricketBallController.SpinType.OffSpin;
        Debug.Log("Selected Spin subtype: OffSpin");
    }

    public void SelectLegSpin()
    {
        selectedSpinType = CricketBallController.SpinType.LegSpin;
        Debug.Log("Selected Spin subtype: LegSpin");
    }

    // =========================
    // Bowl Button
    // =========================
    public void BowlButton()
    {
        if (canBowl) Bowl();
    }

    void Bowl()
    {
        canBowl = false;
        bowlingMeter.StopMeter();

        float movementPercent = bowlingMeter.GetMovementPercent();

        currentBall = Instantiate(ballPrefab, bowlPoint.position, Quaternion.identity);

        // Apply selected type + subtype
        currentBall.ballType = selectedBallType;
        currentBall.swingType = selectedSwingType;
        currentBall.spinType = selectedSpinType;

        // Debug which ball is being bowled
        if (currentBall.ballType == CricketBallController.BallType.Swing)
            Debug.Log($"Bowling Swing: {currentBall.swingType}");
        else if (currentBall.ballType == CricketBallController.BallType.Spin)
            Debug.Log($"Bowling Spin: {currentBall.spinType}");

        Vector3 start = bowlPoint.position;
        Vector3 target = marker.transform.position;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float flightTime = 1.2f;

        Vector3 displacement = target - start;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);

        Vector3 velocityXZ = displacementXZ / flightTime;
        float velocityY = (displacement.y / flightTime) + 0.5f * gravity * flightTime;

        Vector3 velocity = velocityXZ + Vector3.up * velocityY;

        if (currentBall.ballType == CricketBallController.BallType.Swing)
            currentBall.swingStrength = 2.5f * movementPercent;
        else if (currentBall.ballType == CricketBallController.BallType.Spin)
            currentBall.spinStrength = 2f * movementPercent;

        currentBall.trueDirection = displacementXZ.normalized;
        currentBall.LaunchBall(velocity, start, target);

        StartCoroutine(ResetBowling());
    }

    IEnumerator ResetBowling()
    {
        yield return new WaitForSeconds(nextBallDelay);
        canBowl = true;
        bowlingMeter.StartMeter();
    }
}
