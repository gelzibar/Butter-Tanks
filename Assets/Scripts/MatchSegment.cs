using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/// MatchSegment Class
///
/// The MatchSegment class is a parent object for the unique pieces that make up the Match.
/// Is intended to provide a common ancestor so that tracking elements can cycle between
/// each child segment.

public abstract class MatchSegment : NetworkBehaviour
{
    [SyncVar]
    protected bool isCountingDown;

    protected float maxCountdown;
    protected float curCountdown;

    protected MatchManager mm;

    public virtual void Run()
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

    public void ResetTimers(float max)
    {
        maxCountdown = max;
        curCountdown = maxCountdown;
    }

    public float GetMaxCountdown()
    {
        return maxCountdown;
    }

    public float GetCurCountdown()
    {
        return curCountdown;
    }

    public void SetCurCountdown(float newCountdown)
    {
        curCountdown = newCountdown;
    }

    protected void DecreaseTime()
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