using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    RectTransform myRectTransform;
	Text myText;
    Player myPlayer;

    // Use this for initialization
    void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
		myText = transform.parent.Find("Health Text").GetComponent<Text>();

        FindPlayer();
    }

    // Update is called once per frame
    void Update()
    {
		Vector3 localScale = myRectTransform.localScale;
		float scaleY = 0;
        if (myPlayer == null)
        {
            FindPlayer();
        } 

		if(myPlayer != null) 
		{
			scaleY = myPlayer.curHealth / myPlayer.GetMaxHealth();
			scaleY = Mathf.Clamp(scaleY, 0, 1f);
			myText.text = myPlayer.curHealth.ToString();
		}

        myRectTransform.localScale = new Vector3(localScale.x, scaleY, localScale.z);

    }

    void FindPlayer()
    {

        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player instance in players)
        {
            if (instance.GetLocalPlayer())
            {
                myPlayer = instance;
            }
        }
    }
}
