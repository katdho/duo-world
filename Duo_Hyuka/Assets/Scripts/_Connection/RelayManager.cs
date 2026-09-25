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
    private ConnectionUI connectionUI;

    [Header("Pairing")]
    [SerializeField]
    private PairingManager pairingManager;

    private async void Awake() {
        while (NetworkManager.Singleton == null) {
            await Task.Yield();
        }

        transport =
            NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (transport == null) {
            Debug.LogError(
                "RelayManager: UnityTransport is missing!"
            );
            return;
        }

        if (connectionUI == null) {
            connectionUI =
                FindAnyObjectByType<ConnectionUI>();
        }

        if (pairingManager == null) {
            pairingManager =
                FindAnyObjectByType<PairingManager>();
        }

        try {
            await InitializeUnityServices();

            Debug.Log(
                "Unity Services initialized"
            );
        }
        catch (Exception e) {
            Debug.LogError(
                "Unity Services initialization failed:"
            );

            Debug.LogException(e);
        }
    }

    private async Task InitializeUnityServices() {
        if (UnityServices.State !=
            ServicesInitializationState.Initialized) {

            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn) {
            await AuthenticationService.Instance
                .SignInAnonymouslyAsync();
        }

        Debug.Log(
            "Player ID: " +
            AuthenticationService.Instance.PlayerId
        );
    }

    // ---------------------------------------------------------
    // CREATE GAME
    // ---------------------------------------------------------

    public async void CreateGame() {
        Debug.Log("CREATE RELAY GAME");

        try {
            await InitializeUnityServices();

            Allocation allocation =
                await RelayService.Instance
                    .CreateAllocationAsync(1);

            Debug.Log(
                "Relay allocation created"
            );

            string joinCode =
                await RelayService.Instance
                    .GetJoinCodeAsync(
                        allocation.AllocationId
                    );

            Debug.Log(
                "JOIN CODE: " +
                joinCode
            );

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(
                    allocation,
                    "dtls"
                )
            );

            bool success =
                NetworkManager.Singleton.StartHost();

            if (success) {
                Debug.Log(
                    "RELAY HOST STARTED"
                );

                if (connectionUI != null) {
                    connectionUI.SetGameCode(
                        joinCode
                    );

                    connectionUI.SetWaitingStatus(
                        "Waiting for your partner..."
                    );
                }

                if (pairingManager != null) {
                    pairingManager.SendRelayCode(
                        joinCode
                    );
                }
            }
            else {
                Debug.LogError(
                    "FAILED TO START RELAY HOST"
                );

                if (connectionUI != null) {
                    connectionUI.SetWaitingStatus(
                        "Failed to create game."
                    );
                }
            }
        }
        catch (Exception e) {
            Debug.LogError(
                "Create Relay failed:"
            );

            Debug.LogException(e);

            if (connectionUI != null) {
                connectionUI.SetWaitingStatus(
                    "Failed to create game."
                );
            }
        }
    }

    // ---------------------------------------------------------
    // JOIN GAME
    // ---------------------------------------------------------

    public async Task JoinGame(string joinCode) {
        Debug.Log("JOIN RELAY GAME");

        Debug.Log(
            "Join code: " +
            joinCode
        );

        if (string.IsNullOrWhiteSpace(joinCode)) {
            Debug.LogError(
                "Join code is empty!"
            );

            if (connectionUI != null) {
                connectionUI.SetWaitingStatus(
                    "Please enter a game code."
                );
            }

            return;
        }

        try {
            await InitializeUnityServices();

            JoinAllocation allocation =
                await RelayService.Instance
                    .JoinAllocationAsync(
                        joinCode.Trim()
                    );

            Debug.Log(
                "Joined Relay allocation"
            );

            transport.SetRelayServerData(
                AllocationUtils.ToRelayServerData(
                    allocation,
                    "dtls"
                )
            );

            bool success =
                NetworkManager.Singleton.StartClient();

            if (success) {
                Debug.Log(
                    "RELAY CLIENT STARTED"
                );

                if (connectionUI != null) {
                    connectionUI.SetWaitingStatus(
                        "Connecting..."
                    );
                }
            }
            else {
                Debug.LogError(
                    "FAILED TO START RELAY CLIENT"
                );

                if (connectionUI != null) {
                    connectionUI.SetWaitingStatus(
                        "Failed to start connection."
                    );
                }
            }
        }
        catch (Exception e) {
            Debug.LogError(
                "Join Relay failed:"
            );

            Debug.LogException(e);

            if (connectionUI != null) {
                connectionUI.SetWaitingStatus(
                    "Invalid code or connection failed."
                );
            }
        }
    }
}

