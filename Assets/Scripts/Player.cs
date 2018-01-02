using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    /* NOTE
        FixedTimestep in Time Settings and Default Contact offset in Physics were adjusted to smoothly move over tile edges.
     */
    // Game Objects
    private Rigidbody myRigidbody;
    // Physics and Movement
    // W/S Inputs
    float forwardInput;
    // A/D Inputs
    float steeringInput;

    // Acceleration
    public float accelerationMultiplier;
    float forwardMultiplier;
    Vector3 inAirMultiplier;

    // Torque (Angular) Acceleration
    public float pivotMultiplier;

    public Vector3 pivotProduct;
    float pivotCurrent;

    float curMaxVelocity, trueMaxVelocity;
    public Vector3 counterVelocityTotal, counterVelocitySingle;
    float terrainVelocityModifier;
    float squareMaxVelocity;
    float initialMaxAngularVelocity;
    float excessiveVelocityThreshold;

    bool isMaxVelocityExceeded;
    bool hasInitialAdjustmentCompleted;
    int velocityRegulatorCounter, velocityRegulatorCounterMax;

    // Tilt Recovery
    TiltBoundary tiltBoundary;
    bool isTiltRecovering;
    int TiltRecoverySequence;
    int sequence2Counter;
    float initialDegreeX;
    float initialDegreeZ;
    float initialAngularDrag;
    
    // Jumping
    bool isJumping;

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

        // Lowering the center of Mass helps with the stability of the cube. Raising it makes it prone to flip.
        myRigidbody.centerOfMass = new Vector3(myRigidbody.centerOfMass.x, myRigidbody.centerOfMass.y - .15f, myRigidbody.centerOfMass.z);
        myRigidbody.centerOfMass = new Vector3(myRigidbody.centerOfMass.x, myRigidbody.centerOfMass.y - .15f, myRigidbody.centerOfMass.z - .15f);
        //original COM
        // myRigidbody.centerOfMass = new Vector3(myRigidbody.centerOfMass.x, myRigidbody.centerOfMass.y - .5f, myRigidbody.centerOfMass.z);

        // Movement Values
        forwardInput = 0.0f;
        steeringInput = 0.0f;

        // acceleration Multiplier is public and declared from editor
        // pivot Multiplier is public and declared from editor
        forwardMultiplier = accelerationMultiplier;
        inAirMultiplier = new Vector3(5f, 0f, forwardMultiplier * .1f);

        // Turning values
        // pivotProduct is the Total combination of all turning elements. Applied directly to rigidbody through torque
        pivotProduct = new Vector3();
        pivotCurrent = 0f;


        trueMaxVelocity = 35.0f;
        curMaxVelocity = trueMaxVelocity;
        counterVelocityTotal = new Vector3();
        counterVelocitySingle = new Vector3();
        terrainVelocityModifier = 1.0f;
        squareMaxVelocity = curMaxVelocity * curMaxVelocity;
        // Had been set to 3.0f
        initialMaxAngularVelocity = 10.0f;
        myRigidbody.maxAngularVelocity = initialMaxAngularVelocity;

        isMaxVelocityExceeded = false;
        velocityRegulatorCounter = 0;
        velocityRegulatorCounterMax = 90;

        excessiveVelocityThreshold = 2.0f;
        hasInitialAdjustmentCompleted = false;

        // Tilt Recovery
        tiltBoundary = transform.Find("Tilt Boundary").GetComponent<TiltBoundary>();
        isTiltRecovering = false;
        TiltRecoverySequence = 0;
        initialDegreeX = 0;
        initialDegreeZ = 0;
        initialAngularDrag = myRigidbody.angularDrag;

        
        isJumping = false;

    }
    void onFixedUpdate()
    {
        if (BoxCaseGroundCheck())
        {
            Move();
        }
        else if (!BoxCaseGroundCheck() && tiltBoundary.currentCollides == 0)
        {
            if (!isTiltRecovering)
            {
                InAirStability();
            }
            Move2();
        }

        if (isTiltRecovering)
        {
            TiltRecovery();
        }
    }
    void onUpdate()
    {
        curMaxVelocity = trueMaxVelocity * terrainVelocityModifier;
        squareMaxVelocity = curMaxVelocity * curMaxVelocity;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!BoxCaseGroundCheck() && tiltBoundary.currentCollides > 0)
            {
                isTiltRecovering = true;
            }

        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            float offsetX = transform.rotation.eulerAngles.x * -1;
            float offsetZ = transform.rotation.eulerAngles.z * -1;
            transform.Rotate(new Vector3(0, 0, 90f));
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            float offsetX = transform.rotation.eulerAngles.x * -1;
            float offsetZ = transform.rotation.eulerAngles.z * -1;
            transform.Rotate(new Vector3(90f, 0f, 0f));
        }

        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            isJumping = true;
        }
    }

    void TiltRecovery()
    {
        if (TiltRecoverySequence == 0)
        {
            myRigidbody.AddForce(Vector3.up * 8f, ForceMode.Impulse);

            myRigidbody.maxAngularVelocity = 2.0f;
            TiltRecoverySequence++;
            sequence2Counter = 180;
            initialDegreeX = myRigidbody.rotation.eulerAngles.x;
            initialDegreeZ = myRigidbody.rotation.eulerAngles.z;
        }
        else if (TiltRecoverySequence == 1)
        {
            Vector3 curAngle = new Vector3(myRigidbody.rotation.eulerAngles.x, myRigidbody.rotation.eulerAngles.y, myRigidbody.rotation.eulerAngles.z);
            Vector3 localAngularVelocity = transform.InverseTransformVector(myRigidbody.angularVelocity);
            float degreeCeiling = 352f;
            float degreeFloor = 8f;

            float force = .0001f;

            // X section
            int signedX = 1;
            if (curAngle.x < 180)
            {
                signedX *= -1;
            }

            float torqueValue = ExponentialDecay(initialDegreeX, force, (90 - sequence2Counter));
            // Debug.Log("Exp: " + torqueValue.ToString("00.00") + " Counter: " + sequence2Counter + " Vel: " + localAngularVelocity.x + " X: " + curAngle.x);

            if (curAngle.x >= degreeFloor && curAngle.x <= degreeCeiling)
            {
                myRigidbody.AddRelativeTorque((float)(signedX * torqueValue), 0f, 0f, ForceMode.Impulse);
            }

            // Z section
            int signedZ = 1;
            if (curAngle.z < 180)
            {
                signedZ *= -1;
            }

            torqueValue = ExponentialDecay(initialDegreeZ, force, (90 - sequence2Counter));

            if (curAngle.z >= degreeFloor && curAngle.z <= degreeCeiling)
            {
                myRigidbody.AddRelativeTorque(0f, 0f, (float)(signedZ * torqueValue), ForceMode.Impulse);
            }


            // If the unit has righted on both X and Z axes, Increase the drag to kill excess angularVelocity
            if ((curAngle.z <= degreeFloor || curAngle.z >= degreeCeiling) && (curAngle.x <= degreeFloor || curAngle.x >= degreeCeiling))
            {
                myRigidbody.angularDrag = 10.0f;
            }

            if (sequence2Counter <= 0 || BoxCaseGroundCheck())
            {
                myRigidbody.angularDrag = initialAngularDrag;
                myRigidbody.maxAngularVelocity = initialMaxAngularVelocity;
                TiltRecoverySequence = 0;
                isTiltRecovering = false;

            }

            sequence2Counter--;
        }
    }

    void InAirStability()
    {

        if (TiltRecoverySequence == 0)
        {
            // myRigidbody.AddForce(Vector3.up * 8f, ForceMode.Impulse);

            myRigidbody.maxAngularVelocity = 2.0f;
            TiltRecoverySequence++;
            sequence2Counter = 180;
            initialDegreeX = myRigidbody.rotation.eulerAngles.x;
            initialDegreeZ = myRigidbody.rotation.eulerAngles.z;
        }
        else if (TiltRecoverySequence == 1)
        {
            Vector3 curAngle = new Vector3(myRigidbody.rotation.eulerAngles.x, myRigidbody.rotation.eulerAngles.y, myRigidbody.rotation.eulerAngles.z);
            Vector3 localAngularVelocity = transform.InverseTransformVector(myRigidbody.angularVelocity);
            float degreeCeiling = 355f;
            float degreeFloor = 5f;

            float force = .01f;

            // X section
            int signedX = 1;
            if (curAngle.x < 180)
            {
                signedX *= -1;
            }

            float deltaTargetAngularVelocity = 0f;
            float frictionValue = GetComponent<BoxCollider>().material.dynamicFriction;
            float targetValue = .15f;
            float targetedAngularVelocityX = targetValue * signedX;
            deltaTargetAngularVelocity = targetedAngularVelocityX - localAngularVelocity.x;
            // deltaTargetAngularVelocity += frictionValue * Math.Sign(steeringInput);

            Vector3 adjustment = Vector3.right * deltaTargetAngularVelocity;

            // float torqueValue = ExponentialDecay(initialDegreeX, force, (90 - sequence2Counter));
            float torqueValue = force;
            // Debug.Log("Exp: " + torqueValue.ToString("00.00") + " Counter: " + sequence2Counter + " Vel: " + localAngularVelocity.x + " X: " + curAngle.x);

            if (curAngle.x >= degreeFloor && curAngle.x <= degreeCeiling)
            {
                myRigidbody.AddRelativeTorque(adjustment.x, 0f, 0f, ForceMode.VelocityChange);
            }

            // Z section
            int signedZ = 1;
            if (curAngle.z < 180)
            {
                signedZ *= -1;
            }

            float targetedAngularVelocityZ = targetValue * signedZ;
            deltaTargetAngularVelocity = targetedAngularVelocityZ - localAngularVelocity.z;

            // torqueValue = ExponentialDecay(initialDegreeZ, force, (90 - sequence2Counter));
            torqueValue = force;

            adjustment = Vector3.forward * deltaTargetAngularVelocity;

            if (curAngle.z >= degreeFloor && curAngle.z <= degreeCeiling)
            {
                myRigidbody.AddRelativeTorque(0f, 0f, adjustment.z, ForceMode.VelocityChange);
            }


            // If the unit has righted on both X and Z axes, Increase the drag to kill excess angularVelocity
            if ((curAngle.z <= degreeFloor || curAngle.z >= degreeCeiling) && (curAngle.x <= degreeFloor || curAngle.x >= degreeCeiling))
            {
                // myRigidbody.angularDrag = 10.0f;
            }

            if (sequence2Counter <= 0 || BoxCaseGroundCheck())
            {
                myRigidbody.angularDrag = initialAngularDrag;
                myRigidbody.maxAngularVelocity = initialMaxAngularVelocity;
                TiltRecoverySequence = 0;
                //isTiltRecovering = false;

            }

            sequence2Counter--;
        }
    }

    float ExponentialDecay(float initial, float decayRate, float exponent)
    {
        // y = a(1-r)^x
        // initial amount (1-% change) ^ units of time.
        // |
        //  \__

        float solution = initial * (float)Math.Pow((1 - decayRate), exponent);
        return solution;

    }

    void OnGUI()
    {
        Vector3 forwardDirection = new Vector3();
        forwardDirection = Vector3.right * forwardInput * forwardMultiplier;
        GUIStyle style = new GUIStyle();
        style.fontSize = 16;
        style.normal.textColor = Color.red;
        GUI.Label(new Rect(10, 10, 200, 100), "Input: " + steeringInput, style);
        GUI.Label(new Rect(10, 23, 200, 100), "Velocity: " + transform.InverseTransformVector(myRigidbody.velocity), style);
        GUI.Label(new Rect(10, 36, 200, 100), "Angular Velocity: " + transform.InverseTransformVector(myRigidbody.angularVelocity).ToString("00.00"), style);
        GUI.Label(new Rect(10, 49, 200, 100), "Angular Drag: " + myRigidbody.angularDrag, style);
    }

    void Move()
    {

        // Ang Vel of .4-.5 feels good for turning
        // with Vel Change forcemode

        // Forcemode.VelocityChange directly adjusts the angular velocity. Give or take some
        // However, physic material Friction also seems to impact this. Add an additional offset for this

        // Left and right turning is not precisely the same, but it's withing spitting distance. 
        // seems to be something related to friction physics-- setting to zero will ignore it
        // Will be investigating "friction 2" functionality I discovered

        forwardInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        float deltaTargetAngularVelocity = 0f;
        float localVelocityZ = transform.InverseTransformVector(myRigidbody.velocity).z;
        float baseTarget = .75f;
        Vector3 rotation = myRigidbody.rotation.eulerAngles;
        float climbFactor = 1f;
        
        Debug.Log(rotation.x.ToString("00.00"));
        if(rotation.x > 180f && rotation.x < 345f) 
        {
            climbFactor = 0.9f;
        } 
        // Checking it against zero didn't seem to function. checking it against a small value (.25 after viewing vel during attempted rotations)
        // seems to achieve the behavior
        if (Math.Abs(localVelocityZ) >= 0.25f)
        {
            deltaTargetAngularVelocity = Steering2(baseTarget);
        }
        else
        {
            deltaTargetAngularVelocity = Steering2(baseTarget * 2.25f);
        }

        pivotProduct = Vector3.up * deltaTargetAngularVelocity;

        if(isJumping) 
        {
            myRigidbody.AddForce(Vector3.up * 4.5f, ForceMode.VelocityChange);
            isJumping = false;
        }

        if (BoxCaseGroundCheck())
        {
            myRigidbody.AddRelativeTorque(pivotProduct, ForceMode.VelocityChange);
            myRigidbody.AddRelativeForce(Vector3.forward * forwardInput * forwardMultiplier * climbFactor, ForceMode.Force);

            RegulateVelocity();
            RegulateLateralVelocity();
        }
    }

    void Move2()
    {

        // Ang Vel of .4-.5 feels good for turning
        // with Vel Change forcemode

        // Forcemode.VelocityChange directly adjusts the angular velocity. Give or take some
        // However, physic material Friction also seems to impact this. Add an additional offset for this

        // Left and right turning is not precisely the same, but it's withing spitting distance. 
        // seems to be something related to friction physics-- setting to zero will ignore it
        // Will be investigating "friction 2" functionality I discovered

        forwardInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        float deltaTargetAngularVelocity = 0f;
        float localVelocityZ = transform.InverseTransformVector(myRigidbody.velocity).z;
        float baseTarget = .75f;
        // Checking it against zero didn't seem to function. checking it against a small value (.25 after viewing vel during attempted rotations)
        // seems to achieve the behavior
        if (Math.Abs(localVelocityZ) >= 0.25f)
        {
            deltaTargetAngularVelocity = Steering2(baseTarget);
        }
        else
        {
            deltaTargetAngularVelocity = Steering2(baseTarget * 2.25f);
        }

        pivotProduct = Vector3.up * deltaTargetAngularVelocity;

        //if (BoxCaseGroundCheck())
        //{
            myRigidbody.AddRelativeForce(Vector3.right * steeringInput * inAirMultiplier.x, ForceMode.Force);
            myRigidbody.AddRelativeForce(Vector3.forward * forwardInput * inAirMultiplier.z, ForceMode.Force);

            //RegulateVelocity();
            //RegulateLateralVelocity();
        //}
    }

    float Steering(float targetValue)
    {
        float deltaTargetAngularVelocity = 0f;
        if (steeringInput != 0f)
        {
            float frictionValue = GetComponent<BoxCollider>().material.dynamicFriction;
            float localAngularVelocityY = transform.InverseTransformVector(myRigidbody.angularVelocity).y;
            float targetedAngularVelocity = targetValue * Math.Sign(steeringInput);
            deltaTargetAngularVelocity = targetedAngularVelocity - localAngularVelocityY;
            deltaTargetAngularVelocity += frictionValue * Math.Sign(steeringInput);
        }

        return deltaTargetAngularVelocity;
    }

    float Steering2(float targetValue)
    {
        /*
         * For some reason, I'm still seeing a variance on left/right turns. It seems to be something
         * concerning friction values. Removing friction removes the variances.
         * Until I can isolate it further, I've customized the steering function to degrade negative/
         * left turn by 15%-- this seems to get the values in range at the setting angVel goal of .75 
         */
        float deltaTargetAngularVelocity = 0f;
        if (steeringInput != 0f)
        {
            float frictionValue = GetComponent<BoxCollider>().material.dynamicFriction;
            float localAngularVelocityY = transform.InverseTransformVector(myRigidbody.angularVelocity).y;
            float targetedAngularVelocity = targetValue * Math.Sign(steeringInput);
            deltaTargetAngularVelocity = targetedAngularVelocity - localAngularVelocityY;
            if (steeringInput > 0)
            {
                deltaTargetAngularVelocity += frictionValue * Math.Sign(steeringInput);
            }
            else if (steeringInput < 0)
            {
                deltaTargetAngularVelocity += frictionValue * Math.Sign(steeringInput) * .85f;
            }
        }

        return deltaTargetAngularVelocity;
    }

    void RegulateVelocity()
    {
        if (myRigidbody.velocity.sqrMagnitude > squareMaxVelocity)
        {
            float preferredAdjustment = Mathf.Sqrt(myRigidbody.velocity.sqrMagnitude - squareMaxVelocity);
            float overageRatio = (float)System.Math.Round(preferredAdjustment / myRigidbody.velocity.magnitude, 2);
            Vector3 tempVelocity = myRigidbody.velocity.normalized * overageRatio;

            myRigidbody.AddForce(tempVelocity * -1, ForceMode.Impulse);
        }
    }

    void RegulateLateralVelocity()
    {
        Vector3 localVelocity = transform.InverseTransformVector(myRigidbody.velocity);
        // A value that has been working well is .15f
        float maxLateralVelMultiplier = .15f;
        if (Math.Abs(localVelocity.x) > trueMaxVelocity * maxLateralVelMultiplier)
        {
            float maxLateralVelocity = trueMaxVelocity * maxLateralVelMultiplier;
            float squareMaxLateralVelocity = maxLateralVelocity * maxLateralVelocity;
            float preferredAdjustment = localVelocity.x * localVelocity.x - squareMaxLateralVelocity;
            float overageRatio = (float)System.Math.Round(preferredAdjustment / localVelocity.x, 4);
            Vector3 tempVelocity = myRigidbody.velocity.normalized * overageRatio;

            myRigidbody.AddRelativeForce(overageRatio * -1, 0f, 0f, ForceMode.VelocityChange);
        }
    }

    bool BoxCaseGroundCheck()
    {
        Vector3 boxHalfSize = new Vector3(0.45f, 0.45f, .95f);
        int layerMask = 1 << 8;

        layerMask = ~layerMask;
        Vector3 boxStartPosition = new Vector3(myRigidbody.position.x, myRigidbody.position.y, myRigidbody.position.z);
        bool boxResponse = Physics.BoxCast(boxStartPosition, boxHalfSize, transform.up * -1, myRigidbody.rotation, 0.15f, layerMask);
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

    public void AddHealth(int amount)
    {
        //Add Health here
    }
}