using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PrematchManager : NetworkBehaviour
{
    [SyncVar]
    bool isCountingDown;

    MatchManager mm;

    float maxCountdown;
    float curCountdown;

    public void Initialize(MatchManager myMM)
    {
        maxCountdown = 15f;
        curCountdown = maxCountdown;
        mm = myMM;
    }

    // void Update()
    // {

    // }

    public void Run()
    {
        if(isCountingDown)
        {
            DecreaseTime();
        }
        else
        {
            curCountdown = maxCountdown;
        }
    }

    public float GetMaxCountdown()
    {
        return maxCountdown;
    }

    public float GetCurCountdown()
    {
        return curCountdown;
    }

    void DecreaseTime()
    {
        curCountdown -= Time.deltaTime;
        curCountdown = Mathf.Clamp(curCountdown, 0, maxCountdown);
    }

    public void SetIsCountingDown(bool value)
    {
        isCountingDown = value;
    }

    public bool GetIsCountingDown()
    {
        return isCountingDown;
    }

}