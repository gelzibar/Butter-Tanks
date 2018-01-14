using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entitySound : MonoBehaviour
{

    new AudioSource audio;
    public AudioClip sliding;
    public AudioClip jumping;
    public AudioClip wobble;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

	void BasicSound()
    {
        // if (Mathf.Abs(myRigidbody.velocity.z) > 0.01f && BoxCaseGroundCheck())
        // {
        //     audio.clip = sliding;
        //     if (!audio.isPlaying)
        //     {
        //         audio.loop = true;
        //         audio.Play();
        //     }
        // }
        // else if (!BoxCaseGroundCheck() && tiltBoundary.currentCollides == 0)
        // {
        //     audio.clip = wobble;
        //     if (!audio.isPlaying)
        //     { 
        //         audio.loop = true;
        //         audio.Play();
        //     }

        // }
        // else
        // {
        //     audio.Stop();
        // }

        // if (isJumping)
        // {
        //     if (!jumpTrigger)
        //     {
        //         AudioSource.PlayClipAtPoint(jumping, transform.position);
        //         jumpTrigger = true;
        //     }
        // }
        // else
        // {
        //     jumpTrigger = false;
        // }
    }
}
