using UnityEngine;

public class ConversationListener : MonoBehaviour
{
    [Header("References")]
    public PrototypeManager prototypeManager;
    public Transform listeningRing;

    [Header("Speech Detection")]
    public float speechThreshold = 0.02f;
    public float requiredSpeechTime = 3f;

    [Header("Ring")]
    public float minRingScale = 1f;
    public float maxRingScale = 1.25f;
    public float ringSmoothness = 8f;

    private AudioClip microphoneClip;
    private string microphoneDevice;

    private float detectedSpeechTime = 0f;
    private bool isListening = false;
    private bool goalReached = false;

    private Vector3 originalRingScale;

    void Start()
    {
        if (listeningRing != null)
            originalRingScale = listeningRing.localScale;
    }

    public void StartListening()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found.");
            return;
        }

        detectedSpeechTime = 0f;
        goalReached = false;

        microphoneDevice = Microphone.devices[0];

        microphoneClip = Microphone.Start(
            microphoneDevice,
            true,
            10,
            44100
        );

        isListening = true;

        Debug.Log("Microphone listening started.");
    }

    public void StopListening()
    {
        if (!isListening)
            return;

        Microphone.End(microphoneDevice);

        isListening = false;

        if (listeningRing != null)
            listeningRing.localScale = originalRingScale;

        Debug.Log("Microphone listening stopped.");
    }

    void Update()
    {
        if (!isListening || microphoneClip == null)
            return;

        float volume = GetMicrophoneVolume();

        // Detect speech / meaningful audio
        if (volume > speechThreshold)
        {
            detectedSpeechTime += Time.deltaTime;
        }

        UpdateListeningRing(volume);

        // Enough speech detected
        if (detectedSpeechTime >= requiredSpeechTime && !goalReached)
        {
            goalReached = true;

            Debug.Log("CONVERSATION DETECTED");

            StopListening();

            if (prototypeManager != null)
                prototypeManager.ConversationGoalMet();
        }
    }

    float GetMicrophoneVolume()
    {
        int position = Microphone.GetPosition(microphoneDevice);

        if (position < 128)
            return 0f;

        float[] samples = new float[128];

        microphoneClip.GetData(
            samples,
            position - samples.Length
        );

        float total = 0f;

        foreach (float sample in samples)
        {
            total += Mathf.Abs(sample);
        }

        return total / samples.Length;
    }

    void UpdateListeningRing(float volume)
    {
        if (listeningRing == null)
            return;

        float normalizedVolume =
            Mathf.Clamp01(volume / 0.025f);

        float targetScale =
            Mathf.Lerp(
                minRingScale,
                maxRingScale,
                normalizedVolume
            );

        Vector3 target =
            originalRingScale * targetScale;

        listeningRing.localScale =
            Vector3.Lerp(
                listeningRing.localScale,
                target,
                Time.deltaTime * ringSmoothness
            );
    }

    void OnDestroy()
    {
        StopListening();
    }
}
