using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PreroundSegment : MatchSegment
{
    public GameObject prefab_Record;
    bool runOnce;
    bool activeUpdate;

    MatchManager mm;

    public void Initialize(MatchManager myMM)
    {
        ResetTimers(5f);
        mm = myMM;
    }

    void Start()
    {
        prefab_Record = Resources.Load("Player Token") as GameObject;
    }

    void Update()
    {
        if(mm.GetStatus() == Status.Preround)
        {
            activeUpdate = true;
        }
        else
        {
            activeUpdate = false;
        }

        // Determine if preround the the active segment through MatchManager.
        if (activeUpdate)
        {
            OnUpdate();
        }
    }

    void OnUpdate()
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
                    if (parentName != " ")
                    {
                        temp.transform.SetParent(GameObject.Find(parentName).transform);
                        temp.transform.position = temp.transform.parent.position;
                        temp.GetComponent<PlayerToken>().parentName = parentName;
                        NetworkServer.Spawn(temp);
                    }
                    else
                    {
                        Debug.LogWarning("PreroundManager: Parent Object is not ready!");
                    }
                }
                runOnce = true;
            }
        }
    }

    string FirstOpenPosition()
    {
        // T
        // At runtime, null reference related to Connected Slots

        string openPosition = " ";
        if (GameObject.Find("Canvas/Preround/Connected Slots") != null)
        {
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
        }

        return openPosition;
    }

}