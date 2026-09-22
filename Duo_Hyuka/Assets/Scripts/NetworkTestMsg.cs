using UnityEngine;
using Unity.Netcode;
using TMPro;

public class NetworkTestMsg : NetworkBehaviour
{

    public TMP_Text statusText;
    public void SendTest() {
        statusText.text = "Sending test...";
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
        statusText.text = "TEST MESSAGE RECEIVED!";
    }
}