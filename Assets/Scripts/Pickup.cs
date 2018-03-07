using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Pickup : NetworkBehaviour
{
    Transform cross;
    public float rotateIncrement = 1.0f;
    public int healAmount = 10;

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

    void LateUpdate()
    {
        onLateUpdate();
    }

    void onStart()
    {

        cross = transform.Find("Cross").GetComponent<Transform>();


    }
    void onFixedUpdate()
    {

    }
    void onUpdate()
    {
    }

    void onLateUpdate()
    {
        // Used here to supercede anything that occured during physics updates.
        float curAxisAngle = cross.rotation.eulerAngles.y;
        cross.rotation = Quaternion.identity;
        cross.rotation = Quaternion.AngleAxis(curAxisAngle + rotateIncrement, Vector3.up);

    }

    void OnTriggerEnter(Collider col)
    {
        if (isServer)
        {
            // I don't like health being added on the pickup.
            if (col.gameObject.tag == "Player")
            {
                // col.gameObject.GetComponent<Player>().AddHealth(healAmount);
                // Destroy(gameObject);
            }
        }
    }
}
