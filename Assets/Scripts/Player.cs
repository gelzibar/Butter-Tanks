using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct InitialConfiguration
    {
        public Transform player;
        public Vector3 position;
        public Quaternion rotation;

        public void Initialize()
        {
            player = GameObject.FindObjectOfType<Player>().transform;
            position = player.position;
            rotation = player.rotation;
        }

    }
    /* NOTE
        FixedTimestep in Time Settings and Default Contact offset in Physics were adjusted to smoothly move over tile edges.
        Default Timestep .02 seems to generate edge catches regularly
        No instances noticed at Timestep: 0.01 - 0.018.
        Geometry Edge Catches noticed: 0.019 - .020.
     */
    // Game Objects
    private Rigidbody myRigidbody;
    // Physics and Movement
    float forwardInput;
    float steeringInput;

    // Acceleration
    public float accelerationMultiplier;
    float forwardMultiplier;
    Vector3 inAirMultiplier;

    // Torque (Angular) Acceleration
    public Vector3 pivotProduct;

    float curMaxVelocity, trueMaxVelocity;
    float squareMaxVelocity;
    public float initialMaxAngularVelocity;

    // Tilt Recovery
    TiltBoundary tiltBoundary;
    Stabilizer stabilizer;

    bool isJumping;
    bool isBraking;

    // Initial Values
    public InitialConfiguration initial;

    bool jumpTrigger = false;


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
        // Make sure game continues running when not in focus (this belongs somewhere else)
        Application.runInBackground = true;

        // Game Objects
        myRigidbody = GetComponent<Rigidbody>();

        initial.Initialize();

        // Lowering the center of Mass helps with the stability of the cube. Raising it makes it prone to flip.
        myRigidbody.centerOfMass = new Vector3(myRigidbody.centerOfMass.x, myRigidbody.centerOfMass.y - .15f, myRigidbody.centerOfMass.z);
        myRigidbody.centerOfMass = new Vector3(myRigidbody.centerOfMass.x, myRigidbody.centerOfMass.y - .15f, myRigidbody.centerOfMass.z - .15f);

        // Movement Values
        forwardInput = 0;
        steeringInput = 0;

        // acceleration Multiplier is public and declared from editor
        // pivot Multiplier is public and declared from editor
        forwardMultiplier = accelerationMultiplier;
        inAirMultiplier = new Vector3(5f, 0f, forwardMultiplier * .1f);

        // Turning values
        // pivotProduct is the Total combination of all turning elements. Applied directly to rigidbody through torque
        pivotProduct = new Vector3();

        trueMaxVelocity = 35.0f;
        curMaxVelocity = trueMaxVelocity;
        squareMaxVelocity = curMaxVelocity * curMaxVelocity;

        // Had been set to 3.0f
        initialMaxAngularVelocity = 10.0f;
        myRigidbody.maxAngularVelocity = initialMaxAngularVelocity;

        // Tilt Recovery
        tiltBoundary = transform.Find("Tilt Boundary").GetComponent<TiltBoundary>();

        isJumping = false;
        isBraking = false;

        stabilizer = new Stabilizer();
        stabilizer.Initialize(this);

    }
    void onFixedUpdate()
    {
        bool groundCheck = Player.BoxCaseGroundCheck(myRigidbody, transform.up * -1);
        if (groundCheck)
        {
            Move();
        }
        else if (!groundCheck && tiltBoundary.NoCollisions())
        {

            if (!stabilizer.isTiltRecovering)
            {
                //stabilizer.InAirStability();
            }
            Move2();
        }

        if (stabilizer.isTiltRecovering)
        {
            stabilizer.TiltRecovery();
        }
    }
    void onUpdate()
    {
        curMaxVelocity = trueMaxVelocity;
        squareMaxVelocity = curMaxVelocity * curMaxVelocity;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!Player.BoxCaseGroundCheck(myRigidbody, transform.up * -1) && !tiltBoundary.NoCollisions())
            {
                stabilizer.isTiltRecovering = true;
            }

        }
        float rightAngle = 90f;
        if (Input.GetKeyDown(KeyCode.F2))
        {
            transform.Rotate(new Vector3(0, 0, rightAngle));
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            transform.Rotate(new Vector3(rightAngle, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            transform.position = initial.position;
            transform.rotation = initial.rotation;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }
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

        if (rotation.x > 180f && rotation.x < 345f)
        {
            climbFactor = 0.9f;
        }
        // Checking it against zero didn't seem to function. checking it against a small value (.25 after viewing vel during attempted rotations)
        // seems to achieve the behavior
        if (Math.Abs(localVelocityZ) >= 0.25f)
        {
            deltaTargetAngularVelocity = SteeringWithAdjustment(baseTarget);
        }
        else
        {
            deltaTargetAngularVelocity = SteeringWithAdjustment(baseTarget * 2.25f);
        }

        pivotProduct = Vector3.up * deltaTargetAngularVelocity;

        if (isJumping)
        {
            myRigidbody.AddForce(Vector3.up * 4.5f, ForceMode.VelocityChange);
            isJumping = false;
        }

        myRigidbody.AddRelativeTorque(pivotProduct, ForceMode.VelocityChange);
        myRigidbody.AddRelativeForce(Vector3.forward * forwardInput * forwardMultiplier * climbFactor, ForceMode.Force);
        if (isBraking)
        {
            if (Mathf.Abs(localVelocityZ) > 10f)
            {
                myRigidbody.AddRelativeForce(0, 0, localVelocityZ * -.0125f, ForceMode.VelocityChange);
            }
            else if (Mathf.Abs(localVelocityZ) > 2.5f)
            {
                myRigidbody.AddRelativeForce(0, 0, localVelocityZ * -.05f, ForceMode.VelocityChange);
            }
            else if (Mathf.Abs(localVelocityZ) <= 2.5f)
            {
                myRigidbody.AddRelativeForce(0, 0, localVelocityZ * -.85f, ForceMode.VelocityChange);
            }
        }

        RegulateVelocity();
        RegulateLateralVelocity();

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
            deltaTargetAngularVelocity = SteeringWithAdjustment(baseTarget);
        }
        else
        {
            deltaTargetAngularVelocity = SteeringWithAdjustment(baseTarget * 2.25f);
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

    float SteeringWithAdjustment(float targetValue)
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

            myRigidbody.AddRelativeForce(overageRatio * -1, 0f, 0f, ForceMode.VelocityChange);
        }
    }

    public static bool BoxCaseGroundCheck(Rigidbody rb, Vector3 localDown)
    {
        Vector3 boxHalfSize = new Vector3(0.45f, 0.45f, .95f);
        int layerMask = 1 << 8;

        layerMask = ~layerMask;
        Vector3 boxStartPosition = new Vector3(rb.position.x, rb.position.y, rb.position.z);
        bool boxResponse = Physics.BoxCast(boxStartPosition, boxHalfSize, localDown, rb.rotation, 0.15f, layerMask);
        return boxResponse;
    }
}