using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TutorialTextBoxManager))]
public abstract class AbstractTextBoxTrigger : MonoBehaviour
{
    [SerializeField] private string _textBoxContent;
    [Header("Auto Destruction")]
    [SerializeField] private bool _destroyAfterTriggers = false;
    [SerializeField] private int _triggersCountToDestroy = 2;

    private int _currentTriggerCount = 0;

    protected void TriggerTextBox()
    {
        var tutorialTextBoxManager = GetComponent<TutorialTextBoxManager>();
        tutorialTextBoxManager.EnqueueTutorialTextBox(_textBoxContent);

        if (_destroyAfterTriggers)
        {
            _currentTriggerCount++;

            if (_currentTriggerCount >= _triggersCountToDestroy)            
                Destroy(this); // only destroy this component            
        }
    }

    protected void StartTimer(float duration, Action task)
    {
        this.ExecuteAfterTime(duration, task);
    }
}
