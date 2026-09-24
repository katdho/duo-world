using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

public class RelayManager : MonoBehaviour {
    private UnityTransport transport;

    [Header("UI")]
    [SerializeField]
    private DuoConnectUI duoConnectUI;

    private async void Awake() {
        while (NetworkManager.Singleton == null) {
            await Task.Yield();
        }

        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null) {
            Debug.LogError("RelayManager: UnityTransport is missing!");
            return;
        }

        if (duoConnectUI == null) {
            duoConnectUI = FindAnyObjectByType<DuoConnectUI>();
        }

        try {
            await InitializeUnityServices();
            Debug.Log("Unity Services initialized");
        }
        catch (Exception e) {
            Debug.LogError("Unity Services initialization failed:");
            Debug.LogException(e);
        }
    }

    private async Task InitializeUnityServices() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn) {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Debug.Log("Player ID: " + AuthenticationService.Instance.PlayerId);
    }

    public async void CreateGame() {
        Debug.Log("CREATE RELAY GAME");

        try {
            await InitializeUnityServices();

            Allocation allocation =
                await RelayService.Instance.CreateAllocationAsync(1);

            Debug.Log("Relay allocation created");

            string joinCode =
                await RelayService.Instance.GetJoinCodeAsync(
                    allocation.AllocationId
                );

            Debug.Log("JOIN CODE: " + joinCode);

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(
                    allocation,
                    "dtls"
                )
            );

            bool success = NetworkManager.Singleton.StartHost();

            if (success) {
                Debug.Log("RELAY HOST STARTED");

                if (duoConnectUI != null) {
                    duoConnectUI.SetGameCode(joinCode);
                    duoConnectUI.SetWaitingStatus(
                        "Waiting for your partner..."
                    );
                }
            }
            else {
                Debug.LogError("FAILED TO START RELAY HOST");

                if (duoConnectUI != null) {
                    duoConnectUI.SetWaitingStatus(
                        "Failed to create game."
                    );
                }
            }
        }
        catch (Exception e) {
            Debug.LogError("Create Relay failed:");
            Debug.LogException(e);

            if (duoConnectUI != null) {
                duoConnectUI.SetWaitingStatus(
                    "Failed to create game."
                );
            }
        }
    }

    public async Task JoinGame(string joinCode) {
        Debug.Log("JOIN RELAY GAME");
        Debug.Log("Join code: " + joinCode);

        if (string.IsNullOrWhiteSpace(joinCode)) {
            Debug.LogError("Join code is empty!");

            if (duoConnectUI != null) {
                duoConnectUI.SetWaitingStatus(
                    "Please enter a game code."
                );
            }

            return;
        }

        try {
            await InitializeUnityServices();

            JoinAllocation allocation =
                await RelayService.Instance.JoinAllocationAsync(
                    joinCode.Trim()
                );

            Debug.Log("Joined Relay allocation");

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(
                    allocation,
                    "dtls"
                )
            );

            bool success = NetworkManager.Singleton.StartClient();

            if (success) {
                Debug.Log("RELAY CLIENT STARTED");

                if (duoConnectUI != null) {
                    duoConnectUI.SetWaitingStatus("Connecting...");
                }
            }
            else {
                Debug.LogError("FAILED TO START RELAY CLIENT");

                if (duoConnectUI != null) {
                    duoConnectUI.SetWaitingStatus(
                        "Failed to start connection."
                    );
                }
            }
        }
        catch (Exception e) {
            Debug.LogError("Join Relay failed:");
            Debug.LogException(e);

            if (duoConnectUI != null) {
                duoConnectUI.SetWaitingStatus(
                    "Invalid code or connection failed."
                );
            }
        }
    }
}