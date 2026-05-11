using UnityEngine;

public class BowlingMeter : MonoBehaviour
{
    public RectTransform fill;

    [Header("Meter")]
    public float maxHeight = 300f;
    public float speed = 400f;

    private bool movingUp = true;
    private bool isRunning = true;
    private float currentHeight;

    void Update()
    {
        if (!isRunning) return;
        MoveMeter();
    }

    void MoveMeter()
    {
        if (movingUp)
        {
            currentHeight += speed * Time.deltaTime;
            if (currentHeight >= maxHeight) { currentHeight = maxHeight; movingUp = false; }
        }
        else
        {
            currentHeight -= speed * Time.deltaTime;
            if (currentHeight <= 0) { currentHeight = 0; movingUp = true; }
        }

        fill.sizeDelta = new Vector2(fill.sizeDelta.x, currentHeight);
    }

    public void StopMeter() => isRunning = false;
    public void StartMeter() => isRunning = true;

    public float GetMovementPercent()
    {
        float normalized = currentHeight / maxHeight;
        float distance = Mathf.Abs(normalized - 0.5f);
        float movement = Mathf.Clamp01(1f - (distance * 2f));

        float percent = movement * 100f;
        string zone = (percent <= 20f) ? "RED (0%)" :
                      (percent <= 55f) ? "YELLOW (~40%)" :
                      (percent <= 85f) ? "GREEN (~70%)" : "BLUE (100%)";

        Debug.Log(zone + " | Swing/Spin : " + Mathf.RoundToInt(percent) + "%");
        return movement;
    }
}