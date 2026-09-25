using UnityEngine;
using TMPro;

public class DuoConnectUI : MonoBehaviour {
    [Header("Panels")]
    public GameObject homePanel;
    public GameObject joinPanel;
    public GameObject waitingPanel;
    public GameObject tapPanel;

    [Header("Join UI")]
    public TMP_InputField joinCodeInput;

    [Header("Waiting UI")]
    public TMP_Text gameCodeText;
    public TMP_Text waitingText;

    [Header("Tap UI")]
    public TMP_Text tapStatusText;

    [Header("Networking")]
    public RelayManager relayManager;
    public PairingManager pairingManager;

    private void Awake() {
        if (relayManager == null)
            relayManager = FindAnyObjectByType<RelayManager>();

        if (pairingManager == null)
            pairingManager = FindAnyObjectByType<PairingManager>();
    }

    private void Start() {
        ShowHome();
    }

    public void ShowHome() {
        if (pairingManager != null)
            pairingManager.StopPairing();

        SetPanel(homePanel);
    }

    public void ShowJoin() {
        if (pairingManager != null)
            pairingManager.StopPairing();

        SetPanel(joinPanel);

        if (joinCodeInput != null) {
            joinCodeInput.text = "";
            joinCodeInput.Select();
            joinCodeInput.ActivateInputField();
        }
    }

    public void ShowWaiting() {
        SetPanel(waitingPanel);
    }

    public void ShowTap() {
        SetPanel(tapPanel);

        if (tapStatusText != null)
            tapStatusText.text =
                "Waiting for another phone...";

        if (pairingManager != null)
            pairingManager.StartPairing();
    }

    public async void JoinGame() {
        if (relayManager == null) {
            Debug.LogError(
                "DuoConnectUI: RelayManager is not assigned!"
            );
            return;
        }

        if (joinCodeInput == null) {
            Debug.LogError(
                "DuoConnectUI: Join code input is not assigned!"
            );
            return;
        }

        string code =
            joinCodeInput.text.Trim();

        if (string.IsNullOrWhiteSpace(code)) {
            joinCodeInput.Select();
            joinCodeInput.ActivateInputField();
            return;
        }

        ShowWaiting();
        SetWaitingStatus(
            "Joining game..."
        );

        await relayManager.JoinGame(code);
    }

    public void CreateGame() {
        if (relayManager == null) {
            Debug.LogError(
                "DuoConnectUI: RelayManager is not assigned!"
            );
            return;
        }

        ShowWaiting();

        SetWaitingStatus(
            "Creating game..."
        );

        relayManager.CreateGame();
    }

    public void SetGameCode(string code) {
        if (gameCodeText != null)
            gameCodeText.text =
                "Game Code: " + code;
    }

    public void SetWaitingStatus(string message) {
        if (waitingText != null)
            waitingText.text = message;
    }

    public void SetTapStatus(string message) {
        if (tapStatusText != null)
            tapStatusText.text = message;
    }

    private void SetPanel(GameObject activePanel) {
        if (homePanel != null)
            homePanel.SetActive(
                activePanel == homePanel
            );

        if (joinPanel != null)
            joinPanel.SetActive(
                activePanel == joinPanel
            );

        if (waitingPanel != null)
            waitingPanel.SetActive(
                activePanel == waitingPanel
            );

        if (tapPanel != null)
            tapPanel.SetActive(
                activePanel == tapPanel
            );
    }
}