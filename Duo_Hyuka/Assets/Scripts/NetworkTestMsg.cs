using UnityEngine;
using Unity.Netcode;

public class NetworkTestMsg : NetworkBehaviour
{
    public void SendTest() {
        SendTestRpc();
    }

    [Rpc(SendTo.Server)]
    private void SendTestRpc(){
        Debug.Log("TEST MSG RECEIVED BY SERVER");
        SendTestToClientsRpc();
    }

    [Rpc(SendTo.NotServer)]
    private void SendTestToClientsRpc(){
        Debug.Log("TEST MSG RECEIVED BY CLIENT");
    }
}