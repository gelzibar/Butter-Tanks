using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    [System.Serializable]
    public struct PositionSettings
    {
        // Many times, you don't want to be looking at the transform of the target
        // but at some position slightly off from it
        public Vector3 subjectOffset;
        // The goal "default" distance from the subject
        public float distanceFromSubject;

        /* NOT IMPLEMENTED */
        public float lookSmooth;
        public float zoomSmooth;
        public float maxZoom;
        public float minZoom;

        public void Initialize()
        {
            subjectOffset = new Vector3(0, 3.4f, 0);
            lookSmooth = 100f;
            distanceFromSubject = -5;
            zoomSmooth = 10;
            maxZoom = -2;
            minZoom = -15;
        }
    }

    [System.Serializable]
    public class OrbitSettings
    {
        public float xRotation = -20;
        public float yRotation = -180;
        public float maxXrotation = 25;
        public float minXrotation = -85;
        public float vOrbitSmooth = 150;
        public float hOrbitSmooth = 150;
    }

    [System.Serializable]
    public class DebugSettings
    {
        public bool drawDesiredCollisionLines = true;
        public bool drawAdjustedCollisionLines = true;
    }

    public PositionSettings position;
    public OrbitSettings orbit = new OrbitSettings();
    public DebugSettings debug = new DebugSettings();

    // target ~= playerTransform
    public Transform target;
    Transform playerTransform;
    public float xOffset;

    public CollisionHandler collision = new CollisionHandler();

    // Additional Settings

    public bool smoothFollow = true;
    public float smooth = 0.05f;
    public float adjustmentDistance = -8f;
    public Vector3 targetPosition = Vector3.zero;
    Vector3 destination = Vector3.zero;
    Vector3 adjustedDestination = Vector3.zero;

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
        playerTransform = GameObject.FindObjectOfType<Player>().transform;
        xOffset = -18.233f;

        PositionSettings position = new PositionSettings();
        position.Initialize();

        MoveToTarget();

        collision.Initialize(GetComponent<Camera>());
        collision.UpdateClipPoints(transform.position, transform.rotation, ref collision.adjustedClipPoints);
        collision.UpdateClipPoints(destination, transform.rotation, ref collision.desiredClipPoints);

        /* [Instruction]
            Want all lines meetings a player.transform
        */


    }

    void MoveToTarget()
    {
        targetPosition = playerTransform.position + position.subjectOffset;
        // destination = Quaternion.Euler(0, playerTransform.eulerAngles.y, 0) * Vector3.forward * position.distanceFromSubject;
        destination = Quaternion.Euler(0, 0, 0) * Vector3.forward * position.distanceFromSubject;
        // destination += targetPosition;
        destination = playerTransform.TransformPoint(destination + position.subjectOffset);
        if (!collision.isColliding)
        {
            /****** AJH section */
            // Keep the Camera behind the vehicle while it turns
            //transform.position = playerTransform.TransformPoint(cameraOffset);

            //transform.LookAt(playerTransform);
            // Offset rotation to have a sightline similar to cart driver
            //transform.Rotate(xOffset, 0, 0);
            /****** AJH section end */

            // Debug.Log(playerTransform.TransformPoint(destination));
            transform.position = destination;
        }
        else
        {


            adjustedDestination = Quaternion.Euler(0, 0, 0) * -Vector3.forward * adjustmentDistance;
            // adjustedDestination += targetPosition;
            adjustedDestination = playerTransform.TransformPoint(adjustedDestination + position.subjectOffset);

            // Debug.Log(playerTransform.TransformPoint(adjustedDestination));
            transform.position = adjustedDestination;

            // if(smoothFollow)
            // {
            //     transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref cameraVelocity, smooth);
            // }
            // else
            // {
            //     transform.position = destination;
            // }

        }
        transform.LookAt(playerTransform);
        transform.Rotate(xOffset, 0, 0);

    }
    void onFixedUpdate()
    {
        // MoveToTarget();

        collision.UpdateClipPoints(transform.position, transform.rotation, ref collision.adjustedClipPoints);
        collision.UpdateClipPoints(destination, transform.rotation, ref collision.desiredClipPoints);

        collision.CheckColliding(targetPosition);

        adjustmentDistance = collision.GetAdjustedDistanceWithRayFrom(targetPosition);

        MoveToTarget();

        // draw debug lines
        for (int i = 0; i < 5; i++)
        {
            if (debug.drawDesiredCollisionLines)
            {
                Debug.DrawLine(targetPosition, collision.desiredClipPoints[i], Color.white);
            }
            if (debug.drawAdjustedCollisionLines)
            {
                Debug.DrawLine(targetPosition, collision.adjustedClipPoints[i], Color.green);
            }
        }

    }
    void onUpdate()
    {

    }

    void onLateUpdate()
    {

    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 16;
        style.normal.textColor = Color.red;
        // GUI.Label(new Rect(10, 23, 200, 100), "Velocity: " + transform.InverseTransformVector(myRigidbody.velocity), style);
        // GUI.Label(new Rect(10, 36, 200, 100), "Angular Velocity: " + transform.InverseTransformVector(myRigidbody.angularVelocity).ToString("00.00"), style);
        // GUI.Label(new Rect(10, 49, 200, 100), "Distance: " + Vector3.Distance(myRigidbody.position, playerRigidbody.position), style);
    }

    [System.Serializable]
    public class CollisionHandler
    {
        public LayerMask collisionLayer;

        // [HideInInspector]
        public bool isColliding = false;
        [HideInInspector]
        public Vector3[] adjustedClipPoints;
        [HideInInspector]
        public Vector3[] desiredClipPoints;

        Camera camera;

        public void Initialize(Camera camera)
        {
            this.camera = camera;
            adjustedClipPoints = new Vector3[5];
            desiredClipPoints = new Vector3[5];

        }

        public void UpdateClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
        {
            if (!camera)
            {
                Debug.Log("Camera Missing!");
                return;
            }

            intoArray = new Vector3[5];
            // or 3.41f
            float z = camera.nearClipPlane;
            float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
            float y = x / camera.aspect;

            // Top Left
            intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;
            // Top Right
            intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
            // Bottom Left
            intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
            // Bottom Right
            intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;
            // Camera
            intoArray[4] = cameraPosition - camera.transform.forward;
        }

        bool CollisionAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
        {
            bool value = false;
            for (int i = 0; i < clipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
                float distance = Vector3.Distance(clipPoints[i], fromPosition);
                if (Physics.Raycast(ray, distance, collisionLayer))
                {
                    value = true;
                }
            }
            return value;
        }

        public float GetAdjustedDistanceWithRayFrom(Vector3 fromPosition)
        {
            float distance = -1;
            for (int i = 0; i < desiredClipPoints.Length; i++)
            {
                Ray ray = new Ray(fromPosition, desiredClipPoints[i] - fromPosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10f, collisionLayer)) 
                {
                    if (distance == -1)
                    {
                        distance = hit.distance;
                    }
                    else
                    {
                        if (hit.distance < distance)
                        {
                            distance = hit.distance;
                        }
                    }

                }
            }

            if (distance == -1)
            {
                distance = 0;
            }

            return distance;
        }

        public void CheckColliding(Vector3 targetPosition)
        {
            bool collisionStatus = false;
            if (CollisionAtClipPoints(desiredClipPoints, targetPosition))
            {
                collisionStatus = true;
            }
            isColliding = collisionStatus;
        }
    }
}

