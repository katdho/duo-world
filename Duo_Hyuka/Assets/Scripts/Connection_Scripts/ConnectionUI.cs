using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class ConnectionUI : MonoBehaviour {
    [Header("Connection Screens")]
    public GameObject tapScreen;
    public GameObject waitingScreen;
    public GameObject confirmationScreen;
    public GameObject consentScreen;
    public GameObject homeScreen;
    public GameObject joinScreen;

    [Header("Tap Screen")]
    public TMP_Text tapStatusText;

    [Header("Waiting Screen")]
    public TMP_Text waitingText;
    public TMP_Text gameCodeText;

    [Header("Join UI")]
    public TMP_InputField joinCodeInput;

    [Header("Networking")]
    public PairingManager pairingManager;
    public RelayManager relayManager;

    private void Awake() {
        if (pairingManager == null)
            pairingManager = FindAnyObjectByType<PairingManager>();

        if (relayManager == null)
            relayManager = FindAnyObjectByType<RelayManager>();
    }

    private void Start() {
        ShowTap();
    }

    public void ShowTap() {
        if (pairingManager != null)
            pairingManager.StartPairing();

        SetScreen(tapScreen);

        if (tapStatusText != null)
            tapStatusText.text = "Waiting for another phone...";
    }

    public void ShowWaiting() {
        SetScreen(waitingScreen);
    }

    public void ShowConfirmation() {
        if (pairingManager != null)
            pairingManager.StopPairing();

        SetScreen(confirmationScreen);
    }

    public void ShowConsent() {
        SetScreen(consentScreen);
    }

    public void ShowHome() {
        if (pairingManager != null)
            pairingManager.StopPairing();

        SetScreen(homeScreen);
    }

    public void ShowJoin() {
        if (pairingManager != null)
            pairingManager.StopPairing();

        SetScreen(joinScreen);

        if (joinCodeInput != null) {
            joinCodeInput.text = "";
            joinCodeInput.Select();
            joinCodeInput.ActivateInputField();
        }
    }

    public void CreateGame() {
        if (relayManager == null) {
            Debug.LogError("ConnectionUI: RelayManager missing!");
            return;
        }

        SetScreen(waitingScreen);
        SetWaitingStatus("Creating game...");

        relayManager.CreateGame();
    }

    public async void JoinGame() {
        if (relayManager == null) {
            Debug.LogError("ConnectionUI: RelayManager missing!");
            return;
        }

        if (joinCodeInput == null) {
            Debug.LogError("ConnectionUI: Join code input missing!");
            return;
        }

        string code = joinCodeInput.text.Trim();

        if (string.IsNullOrWhiteSpace(code)) {
            joinCodeInput.Select();
            joinCodeInput.ActivateInputField();
            return;
        }

        SetScreen(waitingScreen);
        SetWaitingStatus("Joining game...");

        await relayManager.JoinGame(code);
    }

    public void SetGameCode(string code) {
        if (gameCodeText != null)
            gameCodeText.text = "Game Code: " + code;
    }

    public void SetWaitingStatus(string message) {
        if (waitingText != null)
            waitingText.text = message;
    }

    public void SetTapStatus(string message) {
        if (tapStatusText != null)
            tapStatusText.text = message;
    }

    private void SetScreen(GameObject activeScreen) {
        if (tapScreen != null)
            tapScreen.SetActive(activeScreen == tapScreen);

        if (waitingScreen != null)
            waitingScreen.SetActive(activeScreen == waitingScreen);

        if (confirmationScreen != null)
            confirmationScreen.SetActive(activeScreen == confirmationScreen);

        if (consentScreen != null)
            consentScreen.SetActive(activeScreen == consentScreen);

        if (homeScreen != null)
            homeScreen.SetActive(activeScreen == homeScreen);

        if (joinScreen != null)
            joinScreen.SetActive(activeScreen == joinScreen);
    }
}