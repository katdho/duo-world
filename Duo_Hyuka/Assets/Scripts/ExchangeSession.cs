using UnityEngine;

public enum PlayerRole
{
    PlayerA,
    PlayerB
}

public class ExchangeSession : MonoBehaviour
{
    [Header("Local Player")]
    public PlayerRole localRole = PlayerRole.PlayerA;

    [Header("Shared State - TEMP LOCAL")]
    public bool playerAConnected;
    public bool playerBConnected;

    public bool playerABumped;
    public bool playerBBumped;

    public bool playerAConfirmed;
    public bool playerBConfirmed;

    public bool playerAReady;
    public bool playerBReady;

    public bool playerAFinished;
    public bool playerBFinished;

    public void SetPlayerA()
    {
        localRole = PlayerRole.PlayerA;
    }

    public void SetPlayerB()
    {
        localRole = PlayerRole.PlayerB;
    }

    // ---------- TEST FUNCTIONS ----------

    public void SetLocalReady()
    {
        if (localRole == PlayerRole.PlayerA)
            playerAReady = true;
        else
            playerBReady = true;

        CheckBothReady();
    }

    public void FakeOtherPlayerReady()
    {
        if (localRole == PlayerRole.PlayerA)
            playerBReady = true;
        else
            playerAReady = true;

        CheckBothReady();
    }

    private void CheckBothReady()
    {
        if (playerAReady && playerBReady)
        {
            Debug.Log("BOTH PLAYERS READY");
        }
    }

    public void ResetSession()
    {
        playerAConnected = false;
        playerBConnected = false;

        playerABumped = false;
        playerBBumped = false;

        playerAConfirmed = false;
        playerBConfirmed = false;

        playerAReady = false;
        playerBReady = false;

        playerAFinished = false;
        playerBFinished = false;
    }
}
