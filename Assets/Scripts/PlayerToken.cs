using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerToken : NetworkBehaviour
{

    [SyncVar]
    public int connectionId;

	[SyncVar]
	public string parentName;

    // Use this for initialization
    void Start()
    {
		// if(parentName != null)
		// {
		// 	transform.parent = GameObject.Find(parentName).transform;
		// }
    }

	void Spawn()
	{
		
	}

    // Update is called once per frame
    void Update()
    {
		if(parentName != null && GameObject.Find(parentName) != null)
		{
			transform.SetParent(GameObject.Find(parentName).transform);
			//transform.parent = 
		}

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
