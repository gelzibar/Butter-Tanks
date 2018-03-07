using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyManager : NetworkBehaviour
{

    public GameObject playerSlot;

    IDictionary<int, string> connections;
    int currentKey;

    // Use this for initialization
    void Start()
    {
        if (isServer)
        {
            currentKey = 0;
            connections = new Dictionary<int, string>();
        }

        //GameObject.FindObjectOfType<HillNetworkManager>().SetLobbyManager(this);
        // GameObject.FindObjectOfType<HillNetworkManager>().lm = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ConnectionTriggered(int connectionId)
    {
        if (!isServer)
            return;

        GameObject temp = Instantiate(playerSlot, Vector3.zero, Quaternion.identity);
        temp.GetComponent<PlayerToken>().SetConnectionId(connectionId);
        string parentName = FirstOpenPosition();
        temp.transform.SetParent(GameObject.Find(parentName).transform);
        temp.transform.position = temp.transform.parent.position;

        NetworkServer.Spawn(temp);

    }

    public void DisconnectionTriggered(int connectionId)
    {
        if (!isServer)
            return;

        foreach (PlayerToken player in GameObject.FindObjectsOfType<PlayerToken>())
        {
            if (player.GetConnectionId() == connectionId)
            {
                NetworkServer.Destroy(player.gameObject);
            }
        }

    }

    string FirstOpenPosition()
    {
        string openPosition = " ";

        RectTransform[] positions = GameObject.Find("Connected Slots").GetComponentsInChildren<RectTransform>();

        foreach (RectTransform position in positions)
        {
            if (position.name.Contains("Row"))
            {
                if (position.childCount == 0)
                {
                    openPosition = position.name;
                    break;
                }
            }
        }


        return openPosition;
    }
}
