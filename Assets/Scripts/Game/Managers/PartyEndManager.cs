using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manage event trigger Party Ended
/// </summary>
public class PartyEndManager : MonoBehaviour
{
    bool _isPartyFinished = false;

    System.DateTime _startTime;

    #region MonoBehaviour Callback
    private void Start()
    {
        _startTime = System.DateTime.Now;
    }

    void OnEnable()
    {
        Assert.IsNotNull(UIVictoryManager.Instance);

        UIVictoryManager.Instance.OnLaunchVictoryAnimation.AddListener(SetIsPartyFinishedToTrue);
    }

    void OnDisable()
    {
        if (UIVictoryManager.Instance != null)
        {
            UIVictoryManager.Instance.OnLaunchVictoryAnimation.RemoveListener(SetIsPartyFinishedToTrue);
        }
    }

    void OnDestroy()
    {
        TriggerAnalyticEvent();        
    }
    #endregion

    void SetIsPartyFinishedToTrue(float victoryScreenDuration)
    {
        _isPartyFinished = true;
    }

    void TriggerAnalyticEvent()
    {
        float duration = (float)(System.DateTime.Now - _startTime).TotalSeconds;
        int playersCount = GameManager.Instance.InstantiatedCharactersCount;

        ExtendedAnalytics.SendEvent("Party Ended", new Dictionary<string, object>()
        {
            { "Duration",  duration },
            { "Players Count",  playersCount },
            { "Party Finished", _isPartyFinished },
            { "Kill Goal", GameManager.Instance.Gamemode.ValueForVictory }
        });
    }
}
