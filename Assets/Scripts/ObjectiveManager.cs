using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectiveManager : NetworkBehaviour
{
    public List<SpawnHub> hubs;

    bool test = false;

    void Start()
    {
        if (isServer)
        {
            onStart();
        }
    }

    override public void OnStartServer()
    {
        onStart();
    }
    override public void OnStartClient()
    {
        onStart();
    }

    void Update()
    {
        if(!isServer)
            return;
        onUpdate();
    }
    void onStart()
    {
        CreateHubsList();
    }
    
    void onUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CreateHubsList();
            Trigger();
        }
    }

    void SpawnObjective()
    {

    }

    void CreateHubsList()
    {
        /*
         * The Objective Manager will control all Health Objective Spawn Hubs.
         * This will search the scene and gather all hubs into a list.
         * Other functionality will regularly cycle through this list for commands
         */

        hubs = new List<SpawnHub>();

        SpawnHub[] foundHubs = GameObject.FindObjectsOfType<SpawnHub>();
        foreach (SpawnHub instance in foundHubs)
        {
            instance.CreateNodeList();
            hubs.Add(instance);
        }
    }

    void Trigger()
    {
        foreach (SpawnHub instance in hubs)
        {
            int rando = Random.Range(0, instance.nodes.Count);
            instance.nodes[rando].isAnimating = true;
        }
    }
}
