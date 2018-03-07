using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PreroundManager : NetworkBehaviour
{
    public GameObject prefab_Record;
    bool runOnce;

    void Start()
    {
        prefab_Record = Resources.Load("Player Token") as GameObject;
    }

    void Update()
    {
        if (isServer)
        {
            if (!runOnce)
            {
                for (int i = 0; i < NetworkServer.connections.Count; i++)
                {
                    GameObject temp = Instantiate(prefab_Record, Vector3.zero, Quaternion.identity);
                    //temp.GetComponent<PlayerToken>().SetConnectionId(connectionId);
                    string parentName = FirstOpenPosition();
                    temp.transform.SetParent(GameObject.Find(parentName).transform);
                    temp.transform.position = temp.transform.parent.position;
                    NetworkServer.Spawn(temp);
                }
                runOnce = true;
            }
        }
    }

    string FirstOpenPosition()
    {
        string openPosition = " ";

        RectTransform[] positions = GameObject.Find("Canvas/Preround/Connected Slots").GetComponentsInChildren<RectTransform>();

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