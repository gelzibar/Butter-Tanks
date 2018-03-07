using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchTimerDisplay : MonoBehaviour {

	MatchManager myMatchManager;
	Text myText;

	// Use this for initialization
	void Start () {
		myMatchManager = GameObject.FindObjectOfType<MatchManager>();
		myText = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if(myMatchManager == null)
		{
			myMatchManager = GameObject.FindObjectOfType<MatchManager>();
		}
		else
		{
			myText.text = FormatTime(myMatchManager.GetCurCountdown());
		}
		
	}

	string FormatTime(float time)
	{
		string printable = "";
		int minutes = 0;
		float seconds = 0;
		minutes = (int) (time / 60f);
		seconds = (minutes - (time / 60f)) * 60f;
		printable = minutes + ":" + Mathf.Abs(seconds).ToString("00");
		return printable;

	}
}
