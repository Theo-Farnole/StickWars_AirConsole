using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutTextBoxTrigger : AbstractTextBoxTrigger
{
    [Header("Condition")]
    [SerializeField] private float _timeToCheckCondition = 10;
    [SerializeField] private int _hitCountToTriggerBox;
    [Header("Miscellaneous")]
    [SerializeField] private bool _startTimerOnStart;
    [SerializeField] private bool _startTimerOnShortcutDamage;

    private Shortcut _shortcut;
    private int _hitsOnShortcut;

    private bool _shortcutTimerStarted = false;

    void Start()
    {
        _shortcut = FindObjectOfType<Shortcut>();
        _shortcut.OnDamage.AddListener(OnDamage);

        if (_startTimerOnStart)
        {
            StartTimer(_timeToCheckCondition, OnTimerEnded);
        }
    }

    void OnDamage(Entity victim, int damage)
    {
        if (_startTimerOnShortcutDamage && !_shortcutTimerStarted)
        {
            _shortcutTimerStarted = true;
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
