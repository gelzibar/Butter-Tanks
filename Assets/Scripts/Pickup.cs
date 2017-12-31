using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{


    Transform cross;
    Quaternion interiorRotation;
	float rotateIncrement = 1.0f;

	int healAmount = 10;

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

		// interiorRotation tracks rotation ignoring parent transform. 
		// It's reapplied at the Late Update to supercede anything that occured during physics updates.
        interiorRotation = cross.rotation;

    }
    void onFixedUpdate()
    {

    }
    void onUpdate()
    {
        // Reset the rotation value
        Quaternion zeroRotate = Quaternion.identity;

        // Grab the current angle and then apply some rotation  around Y axis.
        float curAxisAngle = cross.rotation.eulerAngles.y;
        interiorRotation = Quaternion.AngleAxis(curAxisAngle + rotateIncrement, Vector3.up);
    }

	void onLateUpdate(){
		// Used here to supercede anything that occured during physics updates.
		cross.rotation = interiorRotation;
	}

	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<Player>().AddHealth(healAmount);

			Destroy(gameObject);
		}
	}
}
