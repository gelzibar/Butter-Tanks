using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchLaunchDisplay : MonoBehaviour {

	public MatchManager mm;
	Text myText;

	// Use this for initialization
	void Start () {
		Initialize();
		myText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () { 
		if(mm == null)
		{
			Initialize();
		}
		else
		{
			if(mm.GetCurCountdown() == mm.GetMaxCountdown())
			{
				myText.text = "--";
			}
			else
			{
				myText.text = mm.GetCurCountdown().ToString("00");
			}
			
		}
		
	}

	void Initialize()
    {
		if(GameObject.FindObjectOfType<MatchManager>())
		{
        	mm = GameObject.FindObjectOfType<MatchManager>();
		}
    }
}
