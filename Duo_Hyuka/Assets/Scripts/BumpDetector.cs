using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BumpDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public float bumpThreshold = 2.2f;
    public float cooldown = 1.0f;

    [Header("References")]
    public TMP_Text debugText;
    public PrototypeManager prototypeManager;

    [Header("Debug")]
    public bool detectionEnabled = false;

    private float lastBumpTime = -10f;
    private Vector3 previousAcceleration;

    public ExchangeSession exchangeSession;

    void Start()
    {
        // Explicitly enable the phone accelerometer
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            previousAcceleration =
                Accelerometer.current.acceleration.ReadValue();

            if (debugText != null)
                debugText.text = "Accelerometer enabled!";
        }
        else
        {
            if (debugText != null)
                debugText.text = "NO ACCELEROMETER FOUND";

            Debug.LogError("No accelerometer detected.");
        }
    }

    void Update()
    {
        if (!detectionEnabled)
            return;

        if (Accelerometer.current == null)
            return;

        Vector3 currentAcceleration =
            Accelerometer.current.acceleration.ReadValue();

        // Show live sensor values
        if (debugText != null)
        {
            debugText.text =
                $"X: {currentAcceleration.x:F2}\n" +
                $"Y: {currentAcceleration.y:F2}\n" +
                $"Z: {currentAcceleration.z:F2}";
        }

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

        detectionEnabled = false;

        if (debugText != null)
            debugText.text = "BUMP!";

        Handheld.Vibrate();

        if (exchangeSession != null)
        {
            if (exchangeSession.localRole == PlayerRole.PlayerA)
                exchangeSession.playerABumped = true;
            else
                exchangeSession.playerBBumped = true;
        }

        // TEMPORARY single-phone test
        if (prototypeManager != null)
            prototypeManager.GoToConfirmation();

    }

    public void EnableDetection()
    {
        detectionEnabled = true;
        lastBumpTime = -10f;

        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            previousAcceleration =
                Accelerometer.current.acceleration.ReadValue();
        }

        if (debugText != null)
            debugText.text = "Waiting for bump...";
    }

    public void DisableDetection()
    {
        detectionEnabled = false;
    }

    void OnDestroy()
    {
        if (Accelerometer.current != null)
            InputSystem.DisableDevice(Accelerometer.current);
    }
}
