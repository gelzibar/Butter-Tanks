using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltBoundary : MonoBehaviour
{
	public int currentCollides;
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

    }
    void onFixedUpdate()
    {

    }
    void onUpdate()
    {
		if(currentCollides < 0) {
			currentCollides = 0;
		}

    }
	void OnTriggerEnter(Collider collider) 
	{
		currentCollides++;

	}
	void OnTriggerExit(Collider collider) 
	{
		currentCollides--;
	}
}
