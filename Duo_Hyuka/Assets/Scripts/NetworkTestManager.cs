using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class NetworkTestManager : MonoBehaviour {

    private UnityTransport transport;

    [Header("UI")]
    public TMP_Text statusText;

    void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        SetStatus("Disconnected");
    }

    public void StartHost() {
        SetStatus("Starting host...");

        //PC/server 
        transport.SetConnectionData(
            "172.20.20.20",
            7777,
            "0.0.0.0"
        );
        bool success = NetworkManager.Singleton.StartHost();
        if (success) {
            SetStatus("Host started\n Waiting for client...");
            Debug.Log("HOST STARTED");
        } else { 
            SetStatus("Failed to start host");
            Debug.LogError("FAILED TO START HOST");
        }
    }

    public void StartClient()
    {
        SetStatus("Starting client...");

        Debug.Log("=== CLIENT START ===");

        if (NetworkManager.Singleton == null)
        {
            SetStatus("ERROR: NetworkManager is NULL");
            Debug.LogError("NetworkManager.Singleton is NULL");
            return;
        }

        if (transport == null)
        {
            SetStatus("ERROR: UnityTransport is NULL");
            Debug.LogError("UnityTransport is NULL");
            return;
        }

        Debug.Log("NetworkManager found");
        Debug.Log("Transport found");

        transport.SetConnectionData(
            "172.20.20.20",
            7777
        );

        Debug.Log("Transport configured");
        Debug.Log("Address: " + transport.ConnectionData.Address);
        Debug.Log("Port: " + transport.ConnectionData.Port);

        bool success = NetworkManager.Singleton.StartClient();

        Debug.Log("StartClient returned: " + success);

        if (success)
        {
            SetStatus("CLIENT STARTED\nConnecting...");
            Debug.Log("CLIENT STARTED");
        }
        else
        {
            SetStatus("FAILED TO START CLIENT");
            Debug.LogError("FAILED TO START CLIENT");
        }
    }

    private void OnClientConnected(ulong clientId){
        Debug.Log("CLIENT CONNECTED: " + clientId);
        if (clientId == NetworkManager.Singleton.LocalClientId){
            if (NetworkManager.Singleton.IsHost){
                SetStatus("HOST CONNECTED\nWaiting for client...");
            } else {
                SetStatus("CONNECTED TO SERVER");
            }
        }
    }

    private void OnClientDisconnected(ulong clientId){
        Debug.Log("CLIENT DISCONNECTED: " + clientId);
        if (clientId == NetworkManager.Singleton.LocalClientId){
            SetStatus("DISCONNECTED");
        } else {
            SetStatus("CLIENT DISCONNECTED\nWaiting for client...");
        }
    }

    public void SetStatus(string msg){
        if (statusText != null) statusText.text = msg;
        Debug.Log("[STATUS] " + msg);
    }

    private void OnDestroy(){
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
}