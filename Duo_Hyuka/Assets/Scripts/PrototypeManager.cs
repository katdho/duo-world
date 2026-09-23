using UnityEngine;

public class PrototypeManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject devMenu01;
    public GameObject tapScreen02;
    public GameObject confirmationScreen03;
    public GameObject consentScreen04;
    public GameObject conversationStarterA05;
    public GameObject conversationStarterB05;
    public GameObject conversationListening06;
    public GameObject convoGoalMet07;
    public GameObject completeScreen08;
    // public GameObject reflectionScreen;
    // public GameObject learnAgainScreen;

    [Header("PopUps")]
    public GameObject needHelpPopup;

    private GameObject currentScreen;

    void Start()
    {
         ShowScreen(devMenu01);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Restart(){
        ShowScreen(devMenu01);
    }

    public void ShowScreen(GameObject screen)
    {
        // Turn everything off
        devMenu01.SetActive(false);
        tapScreen02.SetActive(false);
        confirmationScreen03.SetActive(false);
        consentScreen04.SetActive(false);
        conversationStarterA05.SetActive(false);
        conversationStarterB05.SetActive(false);
        conversationListening06.SetActive(false);
        convoGoalMet07.SetActive(false);
        completeScreen08.SetActive(false);
        needHelpPopup.SetActive(false);
        // reflectionScreen.SetActive(false);
        // learnAgainScreen.SetActive(false);

        // Turn requested screen on
        screen.SetActive(true);
        currentScreen = screen;
    }

    public void GoToTap()
    {
        ShowScreen(tapScreen02);
    }

    public void GoToConfirmation()
    {
        ShowScreen(confirmationScreen03);
    }

    public void GoToConsent()
    {
        ShowScreen(consentScreen04);
    }

    public void GoToConversation(int index)
    {
        if(index == 1){
            ShowScreen(conversationStarterA05);
        }else{ //2
            ShowScreen(conversationStarterB05);
        }
    }

    public void StartListening()
    {
        ShowScreen(conversationListening06);
    }

    public void ConversationGoalMet()
    {
        ShowScreen(convoGoalMet07);
    }

    public void CompleteExchange()
    {
        ShowScreen(completeScreen08);
    }

    public void NeedHelpPop(){
        ShowScreen(conversationListening06);
        needHelpPopup.SetActive(true);
    }
    public void NeedHelpPopClose(){
        needHelpPopup.SetActive(false);
    }
}
