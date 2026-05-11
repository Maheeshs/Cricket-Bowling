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
        // Default selection
        SelectSwingType();
        SelectInswing();

        bowlingMeter.StartMeter();
    }

    // =========================
    // Ball Type Buttons
    // =========================
    public void SelectSwingType()
    {
        selectedBallType = CricketBallController.BallType.Swing;
        Debug.Log("Ball Type: Swing");
    }

    public void SelectSpinType()
    {
        selectedBallType = CricketBallController.BallType.Spin;
        Debug.Log("Ball Type: Spin");
    }

    // =========================
    // Swing Types
    // =========================
    public void SelectInswing()
    {
        selectedSwingType = CricketBallController.SwingType.Inswing;
        Debug.Log("Swing: Inswing");
    }

    public void SelectOutswing()
    {
        selectedSwingType = CricketBallController.SwingType.Outswing;
        Debug.Log("Swing: Outswing");
    }

    // =========================
    // Spin Types
    // =========================
    public void SelectOffSpin()
    {
        selectedSpinType = CricketBallController.SpinType.OffSpin;
        Debug.Log("Spin: OffSpin");
    }

    public void SelectLegSpin()
    {
        selectedSpinType = CricketBallController.SpinType.LegSpin;
        Debug.Log("Spin: LegSpin");
    }

    // =========================
    // Bowl Button
    // =========================
    public void BowlButton()
    {
        if (canBowl)
            Bowl();
    }

    void Bowl()
    {
        canBowl = false;

        bowlingMeter.StopMeter();

        float movementPercent = bowlingMeter.GetMovementPercent();

        currentBall = Instantiate(
            ballPrefab,
            bowlPoint.position,
            Quaternion.identity
        );

        // Apply selected settings
        currentBall.ballType = selectedBallType;
        currentBall.swingType = selectedSwingType;
        currentBall.spinType = selectedSpinType;

        // Strong visible swing
        currentBall.swingStrength = Mathf.Lerp(2f, 8f, movementPercent);

        // Spin strength
        currentBall.spinStrength = Mathf.Lerp(1f, 5f, movementPercent);

        Vector3 start = bowlPoint.position;
        Vector3 target = marker.transform.position;

        // Launch using curve system
        currentBall.LaunchBall(Vector3.zero, start, target);

        StartCoroutine(ResetBowling());
    }

    IEnumerator ResetBowling()
    {
        yield return new WaitForSeconds(nextBallDelay);

        canBowl = true;

        bowlingMeter.StartMeter();
    }
}