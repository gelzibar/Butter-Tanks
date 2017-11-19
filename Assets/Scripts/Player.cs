using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Game Objects
    private Rigidbody myRigidbody;
    // Physics and Movement
    // W/S Inputs
    float forwardInput;
    // Q/E Inputs
    float lateralInput;
    // A/D Inputs
    float pivotInput, pivotInput2;

    // Acceleration
    public float accelerationMultiplier;
    float forwardMultiplier;
    public float lateralMultiplier;

    // Torque (Angular) Acceleration
    public float pivotMultiplier;

    Vector3 pivotProduct;

    float curMaxVelocity, trueMaxVelocity;
    public Vector3 counterVelocityTotal, counterVelocitySingle;
    float terrainVelocityModifier;
    float squareMaxVelocity;
    float maxAngularVelocity;
    float excessiveVelocityThreshold;

    bool isMaxVelocityExceeded;
    bool hasInitialAdjustmentCompleted;
    int velocityRegulatorCounter, velocityRegulatorCounterMax;

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
        myRigidbody.centerOfMass = new Vector3(myRigidbody.centerOfMass.x, myRigidbody.centerOfMass.y - .5f, myRigidbody.centerOfMass.z);

        // Movement Values
        forwardInput = 0.0f;
        lateralInput = 0.0f;
        pivotInput = 0.0f;
        pivotInput2 = 0.0f;

        // acceleration Multiplier is public and declared from editor
        //accelerationMultiplier = 25.0f;
        forwardMultiplier = accelerationMultiplier;
        lateralMultiplier = accelerationMultiplier * 0.6f;
        // pivot Multiplier is public and declared from editor
        //pivotMultiplier = 20.0f;

        pivotProduct = new Vector3();

        trueMaxVelocity = 20.0f;
        curMaxVelocity = trueMaxVelocity;
        counterVelocityTotal = new Vector3();
        counterVelocitySingle = new Vector3();
        terrainVelocityModifier = 1.0f;
        squareMaxVelocity = curMaxVelocity * curMaxVelocity;
        maxAngularVelocity = 2.0f;
        myRigidbody.maxAngularVelocity = maxAngularVelocity;

        isMaxVelocityExceeded = false;
        velocityRegulatorCounter = 0;
        velocityRegulatorCounterMax = 90;

        excessiveVelocityThreshold = 2.0f;
        hasInitialAdjustmentCompleted = false;

    }
    void onFixedUpdate()
    {
        Move();
    }
    void onUpdate()
    {
        lateralMultiplier = accelerationMultiplier * 0.6f;

        curMaxVelocity = trueMaxVelocity * terrainVelocityModifier;

        squareMaxVelocity = curMaxVelocity * curMaxVelocity;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            float offsetX = transform.rotation.eulerAngles.x * -1;
            float offsetZ = transform.rotation.eulerAngles.z * -1;
            transform.Rotate(new Vector3(offsetX, 0, offsetZ));
        }
    }

    void OnGUI()
    {
        Vector3 forwardDirection = new Vector3();
        forwardDirection = Vector3.right * forwardInput * forwardMultiplier;
        // GUI.Label(new Rect(10, 0, 100, 100), forwardInput.ToString());
        // GUI.Label(new Rect(10, 10, 100, 100), lateralInput.ToString());
        // GUI.Label(new Rect(10, 30, 100, 100), squareMaxVelocity.ToString());
        // GUI.Label(new Rect(10, 40, 100, 100), myRigidbody.velocity.sqrMagnitude.ToString());
        // GUI.Label(new Rect(10, 60, 100, 100), counterVelocity.ToString());
        GUI.Label(new Rect(10, 40, 100, 100), myRigidbody.velocity.magnitude.ToString() + ":" + curMaxVelocity);
        GUI.Label(new Rect(10, 50, 100, 100), myRigidbody.velocity.ToString());
        GUI.Label(new Rect(10, 60, 100, 100), myRigidbody.velocity.normalized.ToString());
    }

    void Move()
    {
        forwardInput = Input.GetAxis("Vertical");
        lateralInput = Input.GetAxis("Q/E");
        pivotInput = Input.GetAxis("Horizontal");
        pivotInput2 = Input.GetAxis("Mouse X");

        pivotProduct = Vector3.up * (pivotInput + pivotInput2) * pivotMultiplier;

        if (BoxCaseGroundCheck())
        {
            myRigidbody.AddRelativeTorque(pivotProduct, ForceMode.Force);
            myRigidbody.AddRelativeForce(Vector3.forward * forwardInput * forwardMultiplier, ForceMode.Force);
            myRigidbody.AddRelativeForce(Vector3.right * lateralInput * lateralMultiplier, ForceMode.Force);
            if (myRigidbody.velocity.sqrMagnitude > squareMaxVelocity)
            {
                RegulateVelocity();
                // if (Mathf.Sqrt(myRigidbody.velocity.sqrMagnitude - squareMaxVelocity) > excessiveVelocityThreshold)
                // {
                //     if (!isMaxVelocityExceeded)
                //     {
                //         isMaxVelocityExceeded = true;
                //         counterVelocityTotal = myRigidbody.velocity.normalized * (Mathf.Sqrt(myRigidbody.velocity.sqrMagnitude - squareMaxVelocity) - 2.0f);
                //         counterVelocitySingle = counterVelocityTotal / velocityRegulatorCounterMax;

                //         // myRigidbody.AddRelativeForce(Vector3.back * forwardInput * forwardMultiplier, ForceMode.Force);
                //         myRigidbody.AddForce(counterVelocitySingle * -1, ForceMode.Impulse);
                //         velocityRegulatorCounter++;
                //     }
                //     else if (isMaxVelocityExceeded)
                //     {
                //         if (velocityRegulatorCounter < velocityRegulatorCounterMax)
                //         {
                //             myRigidbody.AddForce(counterVelocitySingle * -1, ForceMode.Impulse);
                //             velocityRegulatorCounter++;
                //         }
                //         else
                //         {
                //             isMaxVelocityExceeded = false;
                //             velocityRegulatorCounter = 0;
                //         }
                //     }
                // }
                // else
                // {
                //     isMaxVelocityExceeded = false;
                //     velocityRegulatorCounter = 0;
                // }

            }

            // myRigidbody.velocity = Vector3.ClampMagnitude(myRigidbody.velocity, maxVelocity);
        }

    }

    void RegulateVelocity()
    {
        float preferredAdjustment = Mathf.Sqrt(myRigidbody.velocity.sqrMagnitude - squareMaxVelocity);
        float overageRatio = (float)System.Math.Round(preferredAdjustment / myRigidbody.velocity.magnitude, 2);
        // if (!hasInitialAdjustmentCompleted)
        // {
        // if (preferredAdjustment > excessiveVelocityThreshold)
        // {
        //     preferredAdjustment = excessiveVelocityThreshold;
        //     hasInitialAdjustmentCompleted = true;
        // }
        // Debug.Log(preferredAdjustment);
        Vector3 tempVelocity = myRigidbody.velocity.normalized * overageRatio;
        // Debug.Log(myRigidbody.velocity + " " + tempVelocity + " " + overageRatio);

        myRigidbody.AddForce(tempVelocity * -1, ForceMode.Impulse);
        // }
    }

    bool BoxCaseGroundCheck()
    {
        // return Physics.SphereCast(transform.position, 0.1f, transform.up * -1, out hit, 1.1f);
        Vector3 boxHalfSize = new Vector3(0.45f, 0.45f, .95f);
        int layerMask = 1 << 8;

        layerMask = ~layerMask;
        Vector3 boxStartPosition = new Vector3(myRigidbody.position.x, myRigidbody.position.y, myRigidbody.position.z);
        bool boxResponse = Physics.BoxCast(boxStartPosition, boxHalfSize, transform.up * -1, myRigidbody.rotation, 0.15f, layerMask);
        Debug.Log(boxResponse);
        return boxResponse;
    }

    void OnCollisionEnter(Collision col)
    {
        float modifier = 1.0f;
        if (col.gameObject.tag == "Hot")
        {
            modifier = 1.4f;
        }
        else if (col.gameObject.tag == "Cold")
        {
            modifier = 0.6f;
        }

        terrainVelocityModifier = modifier;

    }
}
