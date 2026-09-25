using UnityEngine;

public class ListeningPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float minScale = 1f;
    public float maxScale = 1.15f;

    [Header("Speed")]
    public float baseSpeed = 2.5f;
    public float speedVariation = 0.8f;

    [Header("Randomness")]
    public float randomnessAmount = 0.035f;
    public float randomnessSpeed = 1.5f;

    private Vector3 originalScale;
    private float pulseTime = 0f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Slowly vary the pulse speed
        float speedNoise =
            Mathf.PerlinNoise(Time.time * 0.4f, 0f);

        float currentSpeed =
            baseSpeed +
            (speedNoise - 0.5f) * speedVariation;

        pulseTime += Time.deltaTime * currentSpeed;

        // Original smooth sine pulse
        float pulse =
            (Mathf.Sin(pulseTime) + 1f) / 2f;

        float scale =
            Mathf.Lerp(minScale, maxScale, pulse);

        // Add smooth organic randomness
        // PerlinNoise changes continuously, unlike Random.Range
        float noise =
            Mathf.PerlinNoise(
                Time.time * randomnessSpeed,
                10f
            );

        noise = (noise - 0.5f) * 2f;

        scale += noise * randomnessAmount;

        transform.localScale =
            originalScale * scale;
    }

    public void ResetPulse()
    {
        pulseTime = 0f;
        transform.localScale = originalScale;
    }
}