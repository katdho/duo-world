using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip buttonClick;
    public AudioClip phoneTap;
    public AudioClip success;

    public void PlayButtonClick()
    {
        audioSource.PlayOneShot(buttonClick);
    }

    public void PlayPhoneTap()
    {
        audioSource.PlayOneShot(phoneTap);
    }

    public void PlaySuccess()
    {
        audioSource.PlayOneShot(success);
    }
}
