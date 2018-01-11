using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heightGuide : MonoBehaviour
{
    // What to do
    // 1) Have the ball raycast downward
    // 1a) Set an upper limit to the raycast, probably for the best
    // 2) Store the hit information.
    // 3) Take the height guide. Reset its position and scale
    // 4) Get the mid distance between the ball and the hit location.
    // 4a) Apply the position to the height guide position
    // 5) Get the distance between the ball and the hit location
    // 5) Apply the distance to the scale of the height guide

    [System.Serializable]
    public struct Properties
    {
        public Vector3 origin;
        public Vector3 direction;
        public RaycastHit hit;
        public float maxDistance;
        public LayerMask layerMask;

        public void Initialize(Vector3 parentPos)
        {
            origin = parentPos;
            direction = Vector3.down;
            maxDistance = Mathf.Infinity;
            layerMask = 1 << 0;
        }
    }

    public Properties properties;
    MeshRenderer mr;

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
        properties = new Properties();
        properties.Initialize(transform.parent.position);
        mr = GetComponent<MeshRenderer>();
    }
    void onFixedUpdate()
    {
        float offsetY = transform.parent.position.y - ((transform.parent.localScale.y / 2f) * .99f);
        properties.origin = new Vector3(transform.parent.position.x, offsetY, transform.parent.position.z);
        Physics.Raycast(properties.origin, properties.direction, out properties.hit, properties.maxDistance, properties.layerMask);

    }
    void onUpdate()
    {

    }

    void onLateUpdate()
    {
        //position
        float distance = (properties.origin.y - properties.hit.point.y) / 2f;
        float adjustedY = properties.origin.y - distance;
        Transform parent = transform.parent;
        transform.position = new Vector3(parent.position.x, adjustedY, parent.position.z);
        // scale- growth should only be vertical. Maintain x and z scale
        // All scaling is relative to the parent scale size (current 2f). Divide by the parent's scale to get req value.
        transform.localScale = new Vector3(transform.localScale.x, distance / parent.localScale.y, transform.localScale.z);
        // rotation- No desire for the height guide to conform to rotation of its parent. 
        // Keep it set to zero each frame
        transform.rotation = Quaternion.identity;

        // If the scale is very small, disable rendering
        float minThreshold = .005f;
        if (transform.localScale.y <= minThreshold)
        {
            mr.enabled = false;
        }
        else
        {
            mr.enabled = true;
        }
    }
}