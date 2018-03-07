using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownDisplay : MonoBehaviour
{

    ObjectiveManager myObjectiveManager;
    SpawnHub mySpawnHub;
    public GameObject myPlayer;

    void Awake()
    {
        onAwake();
    }

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

    void onAwake()
    {
    }
    void onStart()
    {

    }

    void Initialize()
    {
        myObjectiveManager = GameObject.FindObjectOfType<ObjectiveManager>();
    }
    void onFixedUpdate()
    {

    }

    void SelectClosestObjectiveManager()
    {
        float shortestDistance = -1f;
        float distance = 0f;

        foreach (SpawnHub instance in myObjectiveManager.hubs)
        {
            if (shortestDistance == -1f)
            {
                mySpawnHub = instance;
                shortestDistance = Mathf.Abs(Vector3.Distance(myPlayer.transform.position, mySpawnHub.transform.position));
            }
            else
            {
                // distance1 = Mathf.Abs(Vector3.Distance(myPlayer.transform.position, curObjectiveManager.transform.position));
                distance = Mathf.Abs(Vector3.Distance(myPlayer.transform.position, instance.transform.position));

                if (distance < shortestDistance)
                {
                    mySpawnHub = instance;
                    shortestDistance = distance;
                }

            }
        }

    }
    void onUpdate()
    {
        if(myObjectiveManager == null)
        {
            Initialize();
        }

        foreach (Player player in GameObject.FindObjectsOfType<Player>())
        {
            if (player.GetLocalPlayer())
            {
                myPlayer = player.gameObject;
            }

        }

        if (myPlayer != null)
        {

            SelectClosestObjectiveManager();
            if (mySpawnHub.GetIsCountingDown())
            {
                float countdown = mySpawnHub.GetCountdown();
                // countdown = (float)Math.Round(countdown, 2);
                string textCount = countdown.ToString("00");

                float millisecTransition = 3f;
                if (countdown < millisecTransition)
                {
                    textCount = countdown.ToString("00.0");
                }

                gameObject.GetComponent<Text>().text = textCount;
            }
            else
            {
                gameObject.GetComponent<Text>().text = "--";

            }

        }

    }

}
