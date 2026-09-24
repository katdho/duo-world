using UnityEngine;
using System.Runtime.InteropServices;

public class TextToSpeech : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SpeakText(string text);
#endif

    public static void Speak(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

#if UNITY_IOS && !UNITY_EDITOR
        SpeakText(text);
#else
        // Lets us test the button in Unity Editor
        Debug.Log("TTS would say: " + text);
#endif
    }
}
