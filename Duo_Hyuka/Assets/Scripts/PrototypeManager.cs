using UnityEngine;
using TMPro;

public class PrototypeManager : MonoBehaviour
{
    [Header("Screens")]
    public GameObject devMenu01;
    public GameObject tapScreen02;
    public GameObject confirmationScreen03;
    public GameObject consentScreen04;
    public GameObject conversationStarterA_B;
    public TMP_Text userindicator;
    public TMP_Text userindicator2;
    //public GameObject conversationStarterB05;
    public GameObject conversationListening06;
    public GameObject convoGoalMet07;
    public GameObject completeScreen08;
    // public GameObject reflectionScreen;
    // public GameObject learnAgainScreen;

    public BumpDetector bumpDetector;
    public ExchangeSession exchangeSession;

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

    public void RestartPrototype()
    {
        // Go back to your starting screen
        ShowScreen(devMenu01);
    }
    public void GoToConversationStarter()
    {
        ShowScreen(conversationStarterA_B); //A
        if (exchangeSession.localRole == PlayerRole.PlayerA){
            userindicator.text = "[UserA]";
        }
        else{
            userindicator.text = "[UserB]";
        }
    }

    public void ShowScreen(GameObject screen)
    {
        // Turn everything off
        devMenu01.SetActive(false);
        tapScreen02.SetActive(false);
        confirmationScreen03.SetActive(false);
        consentScreen04.SetActive(false);
        conversationStarterA_B.SetActive(false);
        //conversationStarterB05.SetActive(false);
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

            // Reset bump detector
        if (bumpDetector != null)
        {
            bumpDetector.EnableDetection();
        }
    }

    public void GoToConfirmation()
    {
        ShowScreen(confirmationScreen03);
    }

    public void GoToConsent()
    {
        ShowScreen(consentScreen04);
    }

    public void GoToConversation()
    {
        ShowScreen(conversationStarterA_B);
        //userindicator.text = name;
        // if(index == 1){
        //     ShowScreen(conversationStarterA_B);
        // }else{ //2
        //     ShowScreen(conversationStarterA_B);
        // }
    }

    public void StartListening()
    {
        ShowScreen(conversationListening06);
        if (exchangeSession.localRole == PlayerRole.PlayerA){
            userindicator2.text = "[UserA]";
        }
        else{
            userindicator2.text = "[UserB]";
        }
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
