using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilizer
{
    /*
        Stabilizer is to be used with the player class in order to correct rotation in air and
        calculate crash recovery.

        It will be utilizing the rigidbody to DIRECTLY IMPACT the player
     */
    Rigidbody rb;
    Transform entity;

    int tiltRecoverySequence;
    int sequence2Counter;
    float initialDegreeX;
    float initialDegreeZ;
    public bool isTiltRecovering;
    float initialAngularDrag;
    float initialMaxAngularVelocity;

    void Start()
    {
        OnStart();
    }
    void OnStart()
    {
        
    }

    public void Initialize(Player player)
    {
        rb = player.gameObject.GetComponent<Rigidbody>();
        entity = player.transform;
        initialMaxAngularVelocity = player.initialMaxAngularVelocity;
        initialAngularDrag = rb.angularDrag;
    }

    public void TiltRecovery()
    {
        switch (tiltRecoverySequence)
        {
            case 0:
                RecoveryStart(true);
                break;
            case 1:
                RecoveryInProgress();
                break;
            case 2:
                RecoveryEnd();
                break;
            default:
                break;
        }
    }

    public void InAirStability()
    {
        switch (tiltRecoverySequence)
        {
            case 0:
                RecoveryStart(false);
                break;
            case 1:
                StabilityInProgress();
                break;
            case 2:
                StabilityEnd();
                break;
            default:
                break;
        }
    }

    void RecoveryStart(bool addForce)
    {
        if (addForce)
        {
            rb.AddForce(Vector3.up * 8f, ForceMode.Impulse);
        }

        rb.maxAngularVelocity = 2.0f;
        tiltRecoverySequence++;
        sequence2Counter = 180;
        initialDegreeX = rb.rotation.eulerAngles.x;
        initialDegreeZ = rb.rotation.eulerAngles.z;
    }

    float CalculateTorque(float startAngle, float curAngle)
    {
        float torque;
        int sign = 1;
        float force = .0001f;
        if (curAngle < 180)
        {
            sign *= -1;
        }
        torque = ExponentialDecay(startAngle, force, (90 - sequence2Counter));
        torque *= sign;

        return torque;
    }

    void RecoveryInProgress()
    {
        Vector3 curAngle = new Vector3(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z);
        Vector3 torque = new Vector3();
        float degreeCeiling = 352f;
        float degreeFloor = 8f;

        if (curAngle.x >= degreeFloor && curAngle.x <= degreeCeiling)
        {
            torque.x = CalculateTorque(initialDegreeX, curAngle.x);
        }
        if (curAngle.z >= degreeFloor && curAngle.z <= degreeCeiling)
        {
            torque.z = CalculateTorque(initialDegreeZ, curAngle.z);            
        }
        rb.AddRelativeTorque(torque, ForceMode.Impulse);

        // If the unit has righted on both X and Z axes, Increase the drag to kill excess angularVelocity
        if ((curAngle.z <= degreeFloor || curAngle.z >= degreeCeiling) && (curAngle.x <= degreeFloor || curAngle.x >= degreeCeiling))
        {
            rb.angularDrag = 10.0f;
        }

        if (sequence2Counter <= 0 || Player.BoxCaseGroundCheck(rb, entity.up * -1))
        {
            tiltRecoverySequence++;
        }

        sequence2Counter--;
    }

    void RecoveryEnd()
    {
        StabilityEnd();
        isTiltRecovering = false;
    }

    void StabilityEnd()
    {
        rb.angularDrag = initialAngularDrag;
        rb.maxAngularVelocity = initialMaxAngularVelocity;
        tiltRecoverySequence = 0;

    }

    void StabilityInProgress()
    {
        Vector3 curAngle = new Vector3(rb.rotation.eulerAngles.x, rb.rotation.eulerAngles.y, rb.rotation.eulerAngles.z);
        Vector3 localAngularVelocity = entity.InverseTransformVector(rb.angularVelocity);
        float degreeCeiling = 355f;
        float degreeFloor = 5f;

        //X section
        int signedX = 1;
        if (curAngle.x < 180)
        {
            signedX *= -1;
        }

        float deltaTargetAngularVelocity = 0f;
        float frictionValue = entity.GetComponent<BoxCollider>().material.dynamicFriction;
        float targetValue = .15f;
        float targetedAngularVelocityX = targetValue * signedX;
        deltaTargetAngularVelocity = targetedAngularVelocityX - localAngularVelocity.x;
        // deltaTargetAngularVelocity += frictionValue * Math.Sign(steeringInput);

        Vector3 adjustment = Vector3.right * deltaTargetAngularVelocity;

        if (curAngle.x >= degreeFloor && curAngle.x <= degreeCeiling)
        {
            rb.AddRelativeTorque(adjustment.x, 0f, 0f, ForceMode.VelocityChange);
        }

        // Z section
        int signedZ = 1;
        if (curAngle.z < 180)
        {
            signedZ *= -1;
        }

        float targetedAngularVelocityZ = targetValue * signedZ;
        deltaTargetAngularVelocity = targetedAngularVelocityZ - localAngularVelocity.z;

        adjustment = Vector3.forward * deltaTargetAngularVelocity;

        if (curAngle.z >= degreeFloor && curAngle.z <= degreeCeiling)
        {
            rb.AddRelativeTorque(0f, 0f, adjustment.z, ForceMode.VelocityChange);
        }


        // If the unit has righted on both X and Z axes, Increase the drag to kill excess angularVelocity
        if ((curAngle.z <= degreeFloor || curAngle.z >= degreeCeiling) && (curAngle.x <= degreeFloor || curAngle.x >= degreeCeiling))
        {
            // rb.angularDrag = 10.0f;
        }

        if (sequence2Counter <= 0 || Player.BoxCaseGroundCheck(rb, entity.up * -1))
        {
            tiltRecoverySequence++;
        }
        sequence2Counter--;
    }

    float ExponentialDecay(float initial, float decayRate, float exponent)
    {
        // y = a(1-r)^x
        // initial amount (1-% change) ^ units of time.
        // |
        //  \__

        float solution = initial * (float)Mathf.Pow((1 - decayRate), exponent);
        return solution;

    }
}