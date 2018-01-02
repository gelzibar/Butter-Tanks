using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{

    public GameObject pickupTemplate;
    //Transform[] nodes;
    List<Transform> nodes;
    GameObject curPickup;

    float maxCountdown, curCountdown;
    bool isCountingDown;

    void Start()
    {
        onStart();
    }

    void FixedUpdate()
    {
        onFixedUpdate();
    }

    void Update()
    {
        onUpdate();
    }
    void onStart()
    {
        nodes = new List<Transform>();

        Transform[] tempNode = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform spawner in tempNode)
        {
            if (spawner.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                nodes.Add(spawner);
            }
        }
        maxCountdown = 10.0f;
        curCountdown = 0f;
        isCountingDown = false;

    }
    void onFixedUpdate()
    {

    }
    void onUpdate()
    {
        if (curPickup == null && !isCountingDown)
        {
            isCountingDown = true;
            curCountdown = maxCountdown;
            foreach (Transform spawner in nodes)
            {
                spawner.gameObject.GetComponent<ParticleSystem>().Stop();
            }
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
                nodes[index].GetComponent<ParticleSystem>().Stop();
                nodes[index].GetComponent<ParticleSystem>().Play();
                curPickup = Instantiate(pickupTemplate, nodes[index].position, nodes[index].rotation);
                curPickup.GetComponent<Rigidbody>().AddRelativeForce(force, ForceMode.Impulse);
                isCountingDown = false;
            }
            else
            {
                curCountdown -= Time.deltaTime;
            }
        }

    }

	public bool GetIsCountingDown() {
		return isCountingDown;
	}

	public float GetCurCountdown() {
		return curCountdown;
	}
}
