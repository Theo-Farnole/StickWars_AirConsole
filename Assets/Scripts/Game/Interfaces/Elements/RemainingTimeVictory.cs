using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RemainingTimeVictory : MonoBehaviour
{
    [SerializeField] private Image _timer;

    void Awake()
    {
        _timer.fillAmount = 0;
    }

    public void StartTimer(float timerDuration)
    {
        _timer.DOFillAmount(1, timerDuration);
    }
}
