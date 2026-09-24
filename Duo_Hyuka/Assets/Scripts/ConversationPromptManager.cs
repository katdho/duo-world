using UnityEngine;
using TMPro;

[System.Serializable]
public class ConversationPrompt
{
    [TextArea(2, 4)]
    public string question;

    public string phrase1;
    public string phrase2;
    public string phrase3;
    public string phrase4;
}

[System.Serializable]
public class PromptUISet
{
    public TMP_Text questionText;

    public TMP_Text phrase1Text;
    public TMP_Text phrase2Text;
    public TMP_Text phrase3Text;
    public TMP_Text phrase4Text;
}

public class ConversationPromptManager : MonoBehaviour
{
    [Header("UI Sets")]
    public PromptUISet[] uiSets;

    [Header("Prompt Content")]
    public ConversationPrompt[] prompts;

    private ConversationPrompt currentPrompt;

    void Start()
    {
        GeneratePrompt();
    }

    // ----------------------------------------
    // GENERATE RANDOM PROMPT
    // ----------------------------------------

    public void GeneratePrompt()
    {
        if (prompts == null || prompts.Length == 0)
        {
            Debug.LogWarning("No conversation prompts added!");
            return;
        }

        int randomIndex = Random.Range(0, prompts.Length);
        currentPrompt = prompts[randomIndex];

        UpdateAllUI();

        Debug.Log("Generated prompt: " + currentPrompt.question);
    }

    // ----------------------------------------
    // UPDATE BOTH SCREENS
    // ----------------------------------------

    private void UpdateAllUI()
    {
        if (currentPrompt == null)
            return;

        foreach (PromptUISet ui in uiSets)
        {
            if (ui.questionText != null)
                ui.questionText.text = currentPrompt.question;

            if (ui.phrase1Text != null)
                ui.phrase1Text.text = currentPrompt.phrase1;

            if (ui.phrase2Text != null)
                ui.phrase2Text.text = currentPrompt.phrase2;

            if (ui.phrase3Text != null)
                ui.phrase3Text.text = currentPrompt.phrase3;

            if (ui.phrase4Text != null)
                ui.phrase4Text.text = currentPrompt.phrase4;
        }
    }

    // ----------------------------------------
    // PHRASE BUTTONS
    // ----------------------------------------

    public void SpeakPhrase1()
    {
        if (currentPrompt != null)
            SpeakPhrase(currentPrompt.phrase1);
    }

    public void SpeakPhrase2()
    {
        if (currentPrompt != null)
            SpeakPhrase(currentPrompt.phrase2);
    }

    public void SpeakPhrase3()
    {
        if (currentPrompt != null)
            SpeakPhrase(currentPrompt.phrase3);
    }

    public void SpeakPhrase4()
    {
        if (currentPrompt != null)
            SpeakPhrase(currentPrompt.phrase4);
    }

    private void SpeakPhrase(string phrase)
    {
        Debug.Log("Speak: " + phrase);

        // Actual iPhone TTS comes later
        TextToSpeech.Speak(phrase);
    }
}
