using UnityEngine;
using Unity.Netcode;

public class NetworkTestManager : MonoBehaviour
{
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public void StartHost() {
        bool success = NetworkManager.Singleton.StartHost();
        if (success) Debug.Log("HOST STARTED");
        else Debug.LogError("FAILED TO START HOST");
    }

    public void StartClient() {
        bool success = NetworkManager.Singleton.StartClient();
        if (success) Debug.Log("CLIENT STARTED");
        else Debug.LogError("FAILED TO START CLIENT");
    }

    private void OnClientConnected(ulong clientId){
        Debug.Log("CLIENT CONNECTED: " + clientId);
    }

    private void OnClientDisconnected(ulong clientId){
        Debug.Log("CLIENT DISCONNECTED: " + clientId);
    }
}