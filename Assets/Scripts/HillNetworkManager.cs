using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HillNetworkManager : NetworkManager
{

    public LobbyManager lm;
	bool firstConnection;
	int connId;

    // Use this for initialization
    void Start()
    {
        // lm is populated when in the lobby manager start condition
    }

    // Update is called once per frame
    void Update()
    {
		if(lm == null)
		{
			lm = GameObject.FindObjectOfType<LobbyManager>();
		}
		if(firstConnection && lm != null)
		{
			lm.ConnectionTriggered(connId);
			firstConnection = false;	
			connId = 0;
		}
    }

	// Only occurs on Server. Only when a player connects
    public override void OnServerConnect(NetworkConnection conn)
    {
		if(lm == null)
		{
			firstConnection = true;
			connId = conn.connectionId;
		}
		else
		{
			lm.ConnectionTriggered(conn.connectionId);
		}
    }

	// Only occurs on Server. Only when a player connects
	public override void OnServerDisconnect(NetworkConnection conn)
	{
		//lm.DisconnectionTriggered(conn.connectionId);
	}

	public void SetLobbyManager(LobbyManager lobby)
	{
		lm = lobby;
	}
}
