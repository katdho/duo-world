using UnityEngine;
using TMPro;

public class BumpDetector : MonoBehaviour
{
    [Header("Detection")]
    public float bumpThreshold = 2.2f;
    public float cooldown = 1.0f;

    [Header("Debug")]
    public TMP_Text debugText;

    private float lastBumpTime = -10f;
    private Vector3 previousAcceleration;

    void Start()
    {
        //Input.gyro.enabled = true;
        previousAcceleration = Input.acceleration;
        if (debugText != null)
            debugText.text = "Waiting for bump...";
    }

    void Update()
    {
        Vector3 currentAcceleration = Input.acceleration;

        // Detect sudden CHANGE in acceleration
        float accelerationChange =
            (currentAcceleration - previousAcceleration).magnitude;

        if (accelerationChange > bumpThreshold &&
            Time.time - lastBumpTime > cooldown)
        {
            DetectBump();
            lastBumpTime = Time.time;
        }

        previousAcceleration = currentAcceleration;
    }

    void DetectBump()
    {
        Debug.Log("BUMP DETECTED");

        if (debugText != null)
            debugText.text = "BUMP!";

        Handheld.Vibrate();

        // Later:
        // NetworkManager.RegisterBump(); //the phone detect the bump at this time > screen progress, play a sound
    }
}
