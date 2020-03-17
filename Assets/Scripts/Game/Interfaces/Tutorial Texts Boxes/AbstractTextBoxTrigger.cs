using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TutorialTextBoxManager))]
public abstract class AbstractTextBoxTrigger : MonoBehaviour
{
    [SerializeField] private string _textBoxContent;
    
    protected void TriggerTextBox()
    {
        var tutorialTextBoxManager = GetComponent<TutorialTextBoxManager>();
        tutorialTextBoxManager.EnqueueTutorialTextBox(_textBoxContent);
    }

    protected void StartTimer(float duration, Action task)
    {
        this.ExecuteAfterTime(duration, task);
    }
}
