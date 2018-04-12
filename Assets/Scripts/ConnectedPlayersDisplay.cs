using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectedPlayersDisplay : MonoBehaviour
{
	MatchManager mm;
	Text myText;

    // Use this for initialization
    void Start()
    {
		myText = GetComponent<Text>();

		//SceneManager.sceneLoaded += this.OnLoadCallback;

    }

    // Update is called once per frame
    void Update()
    {
        // if (SceneInfo.sceneLoaded)
        // {
            if(mm == null)
            {
                if(GameObject.FindObjectOfType<MatchManager>())
                    mm = GameObject.FindObjectOfType<MatchManager>();        
            }
            else
            {
			    myText.text = mm.GetConnectedPlayers().ToString();
            }
        // }

    }

    void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        mm = GameObject.FindObjectOfType<MatchManager>();
    }
}
