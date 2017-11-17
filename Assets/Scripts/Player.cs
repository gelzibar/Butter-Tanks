using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Game Objects
    private Rigidbody myRigidbody;
    // Physics and Movement
    float forward;
    float lateral;
    float pivot;

	public float baseMultiplier;
	float forwardMultiplier;
	public float lateralMultiplier;
	public float pivotMultiplier;

	Vector3 pivotProduct;

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
        // Game Objects
        myRigidbody = GetComponent<Rigidbody>();

        // Movement Values
        forward = 0.0f;
        lateral = 0.0f;
        pivot = 0.0f;

		//baseMultiplier = 25.0f;
		forwardMultiplier = baseMultiplier;
		lateralMultiplier = baseMultiplier * 0.6f;
		//pivotMultiplier = 20.0f;

		pivotProduct = new Vector3();

		myRigidbody.maxAngularVelocity = 2.0f;

    }
    void onFixedUpdate()
    {
        Move();
    }
    void onUpdate()
    {
		lateralMultiplier = baseMultiplier * 0.6f;

		if(Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
    }

    void OnGUI()
    {
		Vector3 forwardDirection = new Vector3();
		forwardDirection = Vector3.right * forward * forwardMultiplier;
        GUI.Label(new Rect(10, 0, 100, 100), forward.ToString());
		GUI.Label(new Rect(10, 10, 100, 100), lateral.ToString());
		GUI.Label(new Rect(10, 20, 100, 100), pivot.ToString());
		GUI.Label(new Rect(10, 40, 100, 100), pivotProduct.ToString());
		GUI.Label(new Rect(10, 50, 100, 100), myRigidbody.velocity.ToString());
    }

    void Move()
    {
        forward = Input.GetAxis("Vertical");
        lateral = Input.GetAxis("Q/E");
        pivot = Input.GetAxis("Horizontal");

		pivotProduct = Vector3.up * pivot * pivotMultiplier;
		myRigidbody.AddRelativeTorque(pivotProduct, ForceMode.Force);
		myRigidbody.AddRelativeForce(Vector3.forward * forward * forwardMultiplier, ForceMode.Force);
		myRigidbody.AddRelativeForce(Vector3.right * lateral * lateralMultiplier, ForceMode.Force);

		float maxVelocity = 10.0f;
		myRigidbody.velocity = Vector3.ClampMagnitude(myRigidbody.velocity, maxVelocity);
		
    }
}
