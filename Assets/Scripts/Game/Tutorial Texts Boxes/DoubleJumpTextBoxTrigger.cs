using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpTextBoxTrigger : AbstractTextBoxTrigger
{
    [Header("Condition")]
    [SerializeField] private int _maxDoubleJumpsCount = 3;
    [SerializeField] private float _timerToReachCondition = 5;

    private Dictionary<CharController, int> _doubleJumpsCount = new Dictionary<CharController, int>();

    void Start()
    {
        StartTimer(_timerToReachCondition, OnTimerEnded);

        GameManager.Instance.OnCharacterSpawn += OnCharacterSpawn;

        // trigger OnCharacterSpawn for already spawned character
        foreach (var kvp in GameManager.Instance.Characters)
        {
            if (kvp.Value != null)
                OnCharacterSpawn(kvp.Value);
        }
    }

    void OnCharacterSpawn(CharController charController)
    {
        _doubleJumpsCount.Add(charController, 0);

        // listen to double jump on character creation
        charController.OnDoubleJump += OnDoubleJump;
    }

    void OnDoubleJump(CharController charController)
    {
        if (!_doubleJumpsCount.ContainsKey(charController))
        {
            _doubleJumpsCount.Add(charController, 0);
        }

        _doubleJumpsCount[charController]++;
    }

    void OnTimerEnded()
    {
        foreach (var kvp in _doubleJumpsCount)
        {
            if (kvp.Value <= _maxDoubleJumpsCount)
            {
                TriggerTextBox();
                return;
            }
        }
    }
}
