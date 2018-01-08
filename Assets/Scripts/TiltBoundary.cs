using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltBoundary : MonoBehaviour
{
    public int currentCollides;
    Vector3 curPos;
    public AudioClip impact;

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
        currentCollides = 0;
        curPos = transform.parent.position;

    }
    void onFixedUpdate()
    {

    }
    void onUpdate()
    {
        if (currentCollides < 0)
        {
            currentCollides = 0;
        }

    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "Player" && collider.tag != "Item")
        {
            currentCollides++;
            if(collider.ClosestPoint(transform.position).y < transform.position.y)
            {
                AudioSource.PlayClipAtPoint(impact, transform.position, 1.5f);
            }
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag != "Player" && collider.tag != "Item")
        {
            currentCollides--;
        }
    }
}
