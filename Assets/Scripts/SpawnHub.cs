using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnHub : NetworkBehaviour
{

    public List<SpawnNode> nodes;
    public GameObject pickupTemplate;
    GameObject curPickup;
    [SyncVar]
    public bool isCountingDown;
    float maxCountdown;
    [SyncVar]
    public float curCountdown;

    MatchManager mm;

    void Start()
    {

    }

    void Initialize()
    {
        mm = GameObject.FindObjectOfType<MatchManager>();
        CreateNodeList();

        isCountingDown = false;
        maxCountdown = 10.0f;
        curCountdown = 0f;
    }

    public override void OnStartServer()
    {
        Initialize();
    }

    public override void OnStartClient()
    {
        if (isServer)
            return;

        Initialize();
    }

    void Update()
    {
        if (!isServer)
            return;

        if (mm.GetStatus() == Status.Round)
        {
            if (curPickup == null && !isCountingDown)
            {
                Reset();
            }

            if (isCountingDown)
            {
                if (curCountdown <= 0f)
                {
                    // Determine random direction
                    Vector3 forceDirection = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                    forceDirection = forceDirection.normalized;
                    // Determine random magnitude
                    float magnitude = Random.Range(0f, 2f);
                    Vector3 force = forceDirection * magnitude;

                    // Spawn a new health object
                    int index = Random.Range(0, nodes.Count);
                    nodes[index].isAnimating = true;
                    curPickup = Instantiate(pickupTemplate, nodes[index].transform.position, nodes[index].transform.rotation);
                    curPickup.transform.parent = nodes[index].transform;
                    curPickup.name = pickupTemplate.name;
                    curPickup.GetComponent<Rigidbody>().AddRelativeForce(force, ForceMode.Impulse);
                    NetworkServer.Spawn(curPickup);

                    isCountingDown = false;
                }
                else
                {
                    curCountdown -= Time.deltaTime;
                }
            }
        }
    }

    public void CreateNodeList()
    {
        nodes = new List<SpawnNode>();

        SpawnNode[] tempNode = gameObject.GetComponentsInChildren<SpawnNode>();
        foreach (SpawnNode spawner in tempNode)
        {
            if (spawner.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                nodes.Add(spawner);
            }
        }
    }

    void Reset()
    {
        isCountingDown = true;
        curCountdown = maxCountdown;
        foreach (SpawnNode spawner in nodes)
        {
            spawner.isAnimating = false;
        }

    }

    public float GetCountdown()
    {
        return curCountdown;
    }

    public bool GetIsCountingDown()
    {
        return isCountingDown;
    }
}
