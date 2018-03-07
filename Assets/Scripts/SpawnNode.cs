using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnNode : NetworkBehaviour
{
    public ParticleSystem myParticles;

    [SyncVar]
    public bool isAnimating;

    // Use this for initialization
    void Start()
    {
        myParticles = GetComponent<ParticleSystem>();
        bool isAnimating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isAnimating)
        {
            myParticles.Play();
        }else
        {
            myParticles.Stop();
        }

    }
}
