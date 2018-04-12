using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/// MatchManager class
///
/// The MatchManager class holds the different match segments, as well as the network aware
/// curStatus. It's intended to be an overarching piece gluing the parts together. While the
/// parts will be doing most of the work.

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
    [SyncVar]
    bool isDeletingPlayer;

    PrematchSegment prematch;
    PreroundSegment preround;
    RoundSegment round;

    GameObject idleCam;

    // PREGAME
    // No Player prefab created - DONE
    // Shows number of players connected - DONE
    // countdown to start - DONE
    // Host may start countdown or immediately launch - DONE
    // - Buttons are in place - DONE
    // - Add logic to launch the match - DONE
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

        prematch = gameObject.AddComponent(typeof(PrematchSegment)) as PrematchSegment;
        prematch.Initialize(this);

        preround = gameObject.AddComponent(typeof(PreroundSegment)) as PreroundSegment;
        preround.Initialize(this);

        round = gameObject.AddComponent(typeof(RoundSegment)) as RoundSegment;
        round.Initialize(this);

        idleCam = GameObject.FindObjectOfType<Camera>().gameObject;

    }

    void InitializeRound()
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

    void TerminateRound()
    {
        isCreatingPlayer = false;
        // Need to reactivatee menu camera
        // Need to delete Follow Camera
        // Need to delete Player Object
        // Perhaps ClientScene.DestroyAllClientObjects?
        // GameObject oldCam = GameObject.FindObjectOfType<Camera>().gameObject;
        // oldCam.SetActive(false);
        idleCam.SetActive(true);
        // Destroy(oldCam);
        foreach (NetworkConnection conn in NetworkServer.connections)
        {
            foreach (PlayerController control in conn.playerControllers)
            {
                if (control.gameObject != null)
                {
                    Player playerScript = control.gameObject.GetComponent<Player>();
                    if (playerScript.IsCameraSet())
                    {
                        control.gameObject.GetComponent<Player>().DestroyCamera();
                    }

                    isDeletingPlayer = true;
                }
            }
        }
    }

    void Update()
    {
        if (isServer)
        {
            connectedPlayers = NetworkServer.connections.Count;
            switch (curStatus)
            {
                case Status.Prematch:
                    prematch.Run();
                    maxCountdown = prematch.GetMaxCountdown();
                    curCountdown = prematch.GetCurCountdown();

                    if (curCountdown == 0)
                    {
                        curStatus = Status.Preround;
                        //InitializeMatch();

                    }
                    break;
                case Status.Preround:
                    preround.SetIsCountingDown(true);
                    preround.Run();
                    maxCountdown = preround.GetMaxCountdown();
                    curCountdown = preround.GetCurCountdown();

                    if (curCountdown == 0)
                    {
                        curStatus = Status.Round;
                        InitializeRound();

                        preround.SetIsCountingDown(false);
                    }
                    break;
                case Status.Round:
                    round.SetIsCountingDown(true);
                    round.Run();

                    maxCountdown = round.GetMaxCountdown();
                    curCountdown = round.GetCurCountdown();
                    // DecreaseTime();

                    if (curCountdown == 0)
                    {
                        //RpcPause();
                        curStatus = Status.Postround;

                        round.SetIsCountingDown(false);
                    }
                    break;
                case Status.Postround:
                    TerminateRound();
                    preround.ResetTimers(5f);
                    prematch.ResetTimers(5f);
                    round.ResetTimers(5f);
                    curStatus = Status.Preround;
                    break;
                case Status.Postmatch:
                    break;
                default:
                    break;
            }

        }

        Debug.Log("Creating: " + isCreatingPlayer + " : Count: " + ClientScene.localPlayers.Count);

        if (isCreatingPlayer && ClientScene.localPlayers.Count == 0)
        {
            Debug.Log("Trying to Add Player");
            ClientScene.AddPlayer(0);
        }

        if(isDeletingPlayer)
        {
            // ClientScene.DestroyAllClientObjects();
            isDeletingPlayer = false;
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

    public void Launch()
    {
        prematch.SetCurCountdown(0);
        prematch.SetIsCountingDown(true);
    }
}
