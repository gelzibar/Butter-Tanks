using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownDisplay : MonoBehaviour
{

    HealthManager myHealthManager;

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
        myHealthManager = GameObject.FindObjectOfType<HealthManager>();
    }
    void onFixedUpdate()
    {

    }
    void onUpdate()
    {
        if (myHealthManager.GetIsCountingDown())
        {
            float countdown = myHealthManager.GetCurCountdown();
            // countdown = (float)Math.Round(countdown, 2);
			string textCount = countdown.ToString("00");

			float millisecTransition = 3f;
			if(countdown < millisecTransition) {
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
