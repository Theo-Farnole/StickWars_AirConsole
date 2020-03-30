using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public static class ExtendedAnalytics
{
    public static void SendEvent(string eventName, IDictionary<string, object> eventData = null)
    {
        var result = AnalyticsEvent.Custom(eventName, eventData);

#if UNITY_EDITOR
        Debug.LogFormat("Analytics # Event {0} fired with result {1}. (param: {2})", eventName, result, eventData);
#endif
    }
}