using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{


    Transform cross, guide;
    Quaternion interiorRotation;
    float guideOffsetY;
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
        guide = transform.Find("Guide").GetComponent<Transform>();

		// interiorRotation tracks rotation ignoring parent transform. 
		// It's reapplied at the Late Update to supercede anything that occured during physics updates.
        interiorRotation = cross.rotation;
        guideOffsetY = guide.transform.position.y - (transform.position.y + (transform.localScale.y / 2));

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
        guide.rotation = interiorRotation;
        Vector3 tempVector = new Vector3(transform.position.x, transform.position.y + guideOffsetY, transform.position.z);
        guide.transform.position = tempVector;

	}

	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<Player>().AddHealth(healAmount);

			Destroy(gameObject);
		}
	}
}
