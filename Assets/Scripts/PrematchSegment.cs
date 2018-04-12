using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PrematchSegment : MatchSegment
{

    MatchManager mm;

    public void Initialize(MatchManager myMM)
    {
        ResetTimers(5f);
        mm = myMM;
    }

    // void Update()
    // {

    // }



}