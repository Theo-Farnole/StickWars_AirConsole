using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutTextBoxTrigger : AbstractTextBoxTrigger
{
    [Header("Condition")]
    [SerializeField] private float _timeToCheckCondition = 10;
    [Space]
    [SerializeField] private Shortcut _shortcut;
    [SerializeField] private int _hitCountToTriggerBox;
    [Header("Miscellaneous")]
    [SerializeField] private bool _startTimerOnStart;
    [SerializeField] private bool _startTimerOnShortcutDamage;

    private int _hitsOnShortcut;

    void Start()
    {
        if (_shortcut == null)
        {
            Debug.LogErrorFormat("Shortcut isn't linked in ShortcutTextBoxTrigger {0}.", name);

            _shortcut = FindObjectOfType<Shortcut>();
        }

        _shortcut.OnDamage += OnDamage;

        if (_startTimerOnStart)
        {
            StartTimer(_timeToCheckCondition, OnTimerEnded);
        }
    }

    void OnDamage(Entity victim, int damage)
    {
        if (_startTimerOnShortcutDamage)
        {
            StartTimer(_timeToCheckCondition, OnTimerEnded);
        }

        _hitsOnShortcut++;
    }

    void OnTimerEnded()
    {
        if (_hitsOnShortcut == _hitCountToTriggerBox)
        {
            TriggerTextBox();
        }
    }
}
