using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthStation : NetworkBehaviour
{

    float rate;
    float timer;
    float maxTime;

    [SyncVar]
    bool isHealing;

    ParticleSystem.MinMaxGradient initialColor;

    // Use this for initialization
    void Start()
    {
        if (!isServer)
            return;

        rate = 10f;
        timer = 0;
        maxTime = 1f;

        isHealing = false;

        ParticleSystem ps = transform.GetComponentInChildren<ParticleSystem>();
        ParticleSystem.MainModule ma = ps.main;
        initialColor = ps.main.startColor ;

        initialColor = transform.GetComponentInChildren<ParticleSystem>().main.startColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHealing)
        {
            ParticleSystem ps = transform.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.MainModule ma = ps.main;
            ma.startColor = initialColor;
        }
        else
        {
            ParticleSystem ps = transform.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.MainModule ma = ps.main;
            ma.startColor = Color.white;
        }

        if (!isServer)
            return;

        if (timer >= maxTime)
        {
            timer = 0;
            isHealing = true;

			ParticleSystem ps = transform.GetComponentInChildren<ParticleSystem>();
            ParticleSystem.MainModule ma = ps.main;
            ma.startColor = initialColor;

        }
        else
        {
            timer += Time.deltaTime;
            isHealing = false;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (!isServer)
            return;

        if (col.tag == "Player")
        {
            col.GetComponent<Player>().StopDecay();
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (!isServer)
            return;

        if (col.tag == "Player" && isHealing)
        {
            col.GetComponent<Player>().curHealth += rate;
            isHealing = false;
            timer = 0f;
        }

    }

    void OnTriggerExit(Collider col)
    {
        if (!isServer)
            return;

        if (col.tag == "Player")
        {
            col.GetComponent<Player>().ResumeDecay();
        }

    }
}
