using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchManager : NetworkBehaviour
{
    [SyncVar]
    Status curStatus;

    // Match Time Properties
    [SyncVar]
    float maxCountdown;
    [SyncVar]
    float curCountdown;
    [SyncVar]
    bool pauseCountdown;
    [SyncVar]
    int connectedPlayers;
    [SyncVar]
    bool isCreatingPlayer;

    PrematchManager prematch;
    PreroundManager preround;

    // PREGAME
    // No Player prefab created - DONE
    // Shows number of players connected - DONE
    // countdown to start - DONE
    // Host may start countdown or immediately launch - IN PROGRESS
    // - Buttons are in place - DONE
    // - Add logic to launch the match
    // EXTRA: When all players are readied, shorten countdown

    // countdown should be configurable

    // Use this for initialization
    void Start()
    {
        if (isServer)
        {
            maxCountdown = 60f * .5f;
            curCountdown = maxCountdown;
            curStatus = Status.Prematch;
            pauseCountdown = true;

        }

        prematch = gameObject.AddComponent(typeof(PrematchManager)) as PrematchManager;
        prematch.Initialize(this);

        preround = gameObject.AddComponent(typeof(PreroundManager)) as PreroundManager;
        

    }

    void InitializeMatch()
    {
        maxCountdown = 60f * .5f;
        curCountdown = maxCountdown;

        isCreatingPlayer = true;

        foreach (NetworkConnection conn in NetworkServer.connections)
        {
            GameObject player = GameObject.FindObjectOfType<HillNetworkManager>().playerPrefab;
            // NetworkServer.AddPlayerForConnection(conn, player, 0);
        }
    }

    void Update()
    {
        Debug.Log(curStatus.ToString());
        if (isServer)
        {
            connectedPlayers = NetworkServer.connections.Count;
            if (curStatus == Status.Round)
            {
                DecreaseTime();

                if (curCountdown == 0)
                {
                    RpcPause();
                }
            }
            else if (curStatus == Status.Prematch)
            {
                prematch.Run();
                maxCountdown = prematch.GetMaxCountdown();
                curCountdown = prematch.GetCurCountdown();

                if (curCountdown == 0)
                {
                    Debug.Log("Countdown complete");
                    curStatus = Status.Preround;
                    //InitializeMatch();
                    
                }

            }
        }

        if(isCreatingPlayer)
        {
            ClientScene.AddPlayer(0);
        }
    }

    void DecreaseTime()
    {
        if (!pauseCountdown)
        {
            curCountdown -= Time.deltaTime;
            curCountdown = Mathf.Clamp(curCountdown, 0, maxCountdown);
        }
    }

    public float GetCurCountdown()
    {
        return curCountdown;
    }

    public float GetMaxCountdown()
    {
        return maxCountdown;
    }

    public Status GetStatus()
    {
        return curStatus;
    }

    public int GetConnectedPlayers()
    {
        return connectedPlayers;
    }

    [ClientRpc]
    public void RpcPause()
    {
        Time.timeScale = 0;
    }

    public void StartCountdown()
    {
        prematch.SetIsCountingDown(true);
    }
}
