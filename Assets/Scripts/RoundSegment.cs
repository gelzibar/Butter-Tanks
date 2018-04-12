using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RoundSegment : MatchSegment
{
    public GameObject prefab_Record;
    bool runOnce;
    bool activeUpdate;

    public void Initialize(MatchManager myMM)
    {
        maxCountdown = 5f;
        curCountdown = maxCountdown;
        mm = myMM;
    }



    void Start()
    {
        prefab_Record = Resources.Load("Player Token") as GameObject;
    }

    public override void Run()
    {
        if (isCountingDown)
        {
            DecreaseTime();
        }
        else
        {
            curCountdown = maxCountdown;
        }
    }

}