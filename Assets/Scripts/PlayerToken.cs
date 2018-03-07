using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerToken : NetworkBehaviour {

	[SyncVar]
	public int connectionId;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int GetConnectionId()
	{
		return connectionId;
	}

	public void SetConnectionId(int id)
	{
		connectionId = id;
	}
}
