using UnityEngine;

public class PairingManager : MonoBehaviour {
    [Header("References")]
    public BumpDetector bumpDetector;
    public RelayManager relayManager;
    public ConnectionUI connectionUI;
    public ExchangeSession exchangeSession;

    [Header("Pairing")]
    public bool pairingEnabled = false;

    [Header("Debug")]
    public string localDeviceId;
    public string remoteDeviceId;
    public bool localBumpDetected;
    public bool remoteBumpDetected;
    public bool pairingInProgress;
    public bool isHost;

    private void Awake() {
        if (bumpDetector == null)
            bumpDetector = FindAnyObjectByType<BumpDetector>();

        if (relayManager == null)
            relayManager = FindAnyObjectByType<RelayManager>();

        if (connectionUI == null)
            connectionUI = FindAnyObjectByType<ConnectionUI>();

        if (exchangeSession == null)
            exchangeSession = FindAnyObjectByType<ExchangeSession>();

        localDeviceId = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Local device ID: " + localDeviceId);
    }

    public void StartPairing() {
        Debug.Log("PAIRING STARTED");

        pairingEnabled = true;
        pairingInProgress = false;
        localBumpDetected = false;
        remoteBumpDetected = false;
        remoteDeviceId = "";
        isHost = false;

        if (exchangeSession != null)
            exchangeSession.ResetPairingState();

        if (bumpDetector != null)
            bumpDetector.EnableDetection();

        if (connectionUI != null)
            connectionUI.SetTapStatus("Waiting for another phone...");

        StartPhoneDiscovery();
    }

    public void StopPairing() {
        Debug.Log("PAIRING STOPPED");

        pairingEnabled = false;
        pairingInProgress = false;
        localBumpDetected = false;
        remoteBumpDetected = false;
        remoteDeviceId = "";
        isHost = false;

        if (bumpDetector != null)
            bumpDetector.DisableDetection();

        StopPhoneDiscovery();
    }

    public void OnLocalBumpDetected() {
        if (!pairingEnabled) return;
        if (pairingInProgress) return;

        Debug.Log("LOCAL BUMP DETECTED");

        localBumpDetected = true;

        if (exchangeSession != null)
            exchangeSession.SetLocalBumped();

        if (connectionUI != null)
            connectionUI.SetTapStatus("Bump detected!\nLooking for your partner...");

        CheckPairingReady();
    }

    private void StartPhoneDiscovery() {
        Debug.Log("START PHONE DISCOVERY");

        // TODO: Start BLE advertising/scanning here.
    }

    private void StopPhoneDiscovery() {
        Debug.Log("STOP PHONE DISCOVERY");

        // TODO: Stop BLE advertising/scanning here.
    }

    public void OnRemoteDeviceDiscovered(string deviceId) {
        if (!pairingEnabled) return;
        if (string.IsNullOrWhiteSpace(deviceId)) return;
        if (deviceId == localDeviceId) return;

        Debug.Log("REMOTE DEVICE DISCOVERED: " + deviceId);

        remoteDeviceId = deviceId;

        // TODO: Exchange bump/pairing information via BLE.
    }

    public void OnRemoteBumpDetected() {
        if (!pairingEnabled) return;

        Debug.Log("REMOTE BUMP DETECTED");

        remoteBumpDetected = true;

        if (exchangeSession != null) {
            if (exchangeSession.localRole == PlayerRole.PlayerA)
                exchangeSession.playerBBumped = true;
            else
                exchangeSession.playerABumped = true;
        }

        CheckPairingReady();
    }

    private void CheckPairingReady() {
        if (!localBumpDetected) return;
        if (!remoteBumpDetected) return;
        if (pairingInProgress) return;

        Debug.Log("BOTH PHONES BUMPED");

        pairingInProgress = true;

        DetermineHost();
    }

    private void DetermineHost() {
        if (string.IsNullOrWhiteSpace(remoteDeviceId)) {
            Debug.LogError("Cannot determine host because remote device ID is missing.");
            pairingInProgress = false;
            return;
        }

        int comparison = string.CompareOrdinal(localDeviceId, remoteDeviceId);

        isHost = comparison < 0;

        Debug.Log("HOST DECISION: " + (isHost ? "LOCAL PHONE" : "REMOTE PHONE"));

        if (isHost) {
            if (exchangeSession != null)
                exchangeSession.SetPlayerA();

            StartAsHost();
        }
        else {
            if (exchangeSession != null)
                exchangeSession.SetPlayerB();

            StartAsClient();
        }
    }

    private void StartAsHost() {
        Debug.Log("THIS PHONE IS HOST");

        if (connectionUI != null)
            connectionUI.SetTapStatus("Creating game...");

        if (relayManager != null)
            relayManager.CreateGame();
        else
            Debug.LogError("PairingManager: RelayManager missing!");
    }

    private void StartAsClient() {
        Debug.Log("THIS PHONE IS CLIENT");

        if (connectionUI != null)
            connectionUI.SetTapStatus("Waiting for connection...");

        // Client waits for host to send Relay code via BLE.
    }

    public void SendRelayCode(string joinCode) {
        if (!isHost) {
            Debug.LogError("SendRelayCode called on a non-host phone.");
            return;
        }

        if (string.IsNullOrWhiteSpace(joinCode)) {
            Debug.LogError("Cannot send empty Relay code.");
            return;
        }

        Debug.Log("SEND RELAY CODE TO OTHER PHONE: " + joinCode);

        // TODO: Send joinCode over BLE.
        // Remote phone should then call ReceiveRelayCode(joinCode).
    }

    public async void ReceiveRelayCode(string joinCode) {
        if (isHost) {
            Debug.LogError("ReceiveRelayCode called on host.");
            return;
        }

        if (string.IsNullOrWhiteSpace(joinCode)) {
            Debug.LogError("Received empty Relay code.");
            return;
        }

        Debug.Log("RECEIVED RELAY CODE: " + joinCode);

        if (connectionUI != null)
            connectionUI.SetTapStatus("Joining game...");

        if (relayManager != null)
            await relayManager.JoinGame(joinCode);
        else
            Debug.LogError("PairingManager: RelayManager missing!");
    }
}
