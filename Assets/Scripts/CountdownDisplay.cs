using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownDisplay : MonoBehaviour
{

    List<HealthManager> myHealthManagers;
    HealthManager curHealthManager;
    GameObject myPlayer;

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
        myHealthManagers = new List<HealthManager>();
        HealthManager[] tempManager = new HealthManager[2];
        tempManager = GameObject.FindObjectsOfType<HealthManager>();
        foreach (HealthManager instance in tempManager)
        {
            myHealthManagers.Add(instance);
        }
        Debug.Log(myHealthManagers.ToString());

        myPlayer = GameObject.FindObjectOfType<Player>().gameObject;

        SelectClosestHealthManager();
    }
    void onFixedUpdate()
    {

    }

    void SelectClosestHealthManager()
    {
        float dist1 = Vector3.Distance(myPlayer.transform.position, myHealthManagers[0].transform.position);
        float dist2 = Vector3.Distance(myPlayer.transform.position, myHealthManagers[1].transform.position);

        if (dist1 < dist2)
        {
            curHealthManager = myHealthManagers[0];
        }
        else
        {
                curHealthManager = myHealthManagers[1];
        }

    }
    void onUpdate()
    {
        SelectClosestHealthManager();

        if (curHealthManager.GetIsCountingDown())
        {
            float countdown = curHealthManager.GetCurCountdown();
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
