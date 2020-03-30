using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SessionTime
{
    public static float _sessionStartTime = -1;

    public static void InitNewSession()
    {
        _sessionStartTime = Time.time;
    }
    
    public static float GetCurrentTimeInSession()
    {
        if (_sessionStartTime == -1)
        {
            Debug.LogErrorFormat("You need to use {0} before use {1} method!", nameof(InitNewSession), nameof(GetCurrentTimeInSession));
            return -1;
        }

        return Time.time - _sessionStartTime;
    }
}
