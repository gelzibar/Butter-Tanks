using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
// using UnityEditor.SceneManagement;

//public class UIStateManager : NetworkBehaviour
public class UIStateManager : NetworkBehaviour
{

    // Make a public variable for each Status found in the Status Enum
    public GameObject prematch, preround, round;
    public GameObject prematch_hostUI;
    public MatchManager mm;
    bool sceneLoaded;

    Status lastStatus = Status.Postmatch;


    void Awake()
    {
        SceneManager.sceneLoaded += this.OnLoadCallback;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= this.OnLoadCallback;
    }

    void Update()
    {
        if (lastStatus != mm.GetStatus())
        {
            Debug.Log("last Status conditional");
            if (mm.GetStatus() == Status.Prematch)
            {
                if (!isServer)
                {
                    Debug.Log("Running host UI check");
                    //prematch_hostUI.GetComponent<CanvasGroup>().interactable = false;
                    prematch_hostUI.SetActive(false);
                }
                else
                {
                    Debug.Log("isServer!");
                    prematch_hostUI.SetActive(true);
                }
            }
            else if (mm.GetStatus() == Status.Preround)
            {
                Debug.Log("Preround UI started");
                // Preround should be the same for everyone. No need for server host distinction

            }
            else if (mm.GetStatus() == Status.Round)
            {
                // prematch_hostUI.SetActive(false);
                round.SetActive(true);
            }
            DisableAllStatusUIBut(mm.GetStatus());

            lastStatus = mm.GetStatus();
        }
    }
    // }


    void DisableAllStatusUIBut(Status curStatus)
    {
        switch (curStatus)
        {
            case Status.Prematch:
                prematch.SetActive(true);
                preround.SetActive(false);
                round.SetActive(false);
                break;
            case Status.Preround:
                prematch.SetActive(false);
                preround.SetActive(true);
                round.SetActive(false);
                break;
            case Status.Round:
            case Status.Postround:
            case Status.Postmatch:
            default:
                break;
        }

    }

    void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        //mm = GameObject.FindObjectOfType<MatchManager>();
        SceneInfo.sceneLoaded = true;
    }
}
