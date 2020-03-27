using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stick to wall by moving horizontally while jumping
// Stick to wall by pressing horizontal buttons while jumping

// TODO: register nb of state to sticked
// TODO: start a timer
// TODO: CheckCondition

public class StickToWallTextBox : AbstractTextBoxTrigger
{
    #region Fields
    [Header("Condition")]
    [SerializeField] private float _timerToReachCondition = 5;
    [SerializeField] private int _maxCountSwitchToStickedForm = 3;

    private Dictionary<CharController, int> _countSwitchToStickedForm = new Dictionary<CharController, int>();
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        StartTimer(_timerToReachCondition, OnTimerEnded);

        // trigger OnCharacterSpawn for already spawned character
        foreach (var kvp in GameManager.Instance.Characters)
        {
            if (kvp.Value != null)
                OnCharacterSpawn(kvp.Value);
        }
    }

    void OnEnable()
    {
        GameManager.Instance.OnCharacterSpawn += OnCharacterSpawn;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCharacterSpawn -= OnCharacterSpawn;

        foreach (var kvp in _countSwitchToStickedForm)
        {
            kvp.Key.OnStateChanged -= OnStateChanged;
        }
    }
    #endregion

    #region Events Handlers
    void OnCharacterSpawn(CharController charController)
    {
        _countSwitchToStickedForm.Add(charController, 0);

        charController.OnStateChanged += OnStateChanged;
    }

    void OnStateChanged(CharController charController, AbstractCharState state)
    {
        if (state is CharStateSticked)
        {
            _countSwitchToStickedForm[charController]++;
        }
    }

    void OnTimerEnded()
    {
        foreach (var kvp in _countSwitchToStickedForm)
        {
            if (kvp.Value <= _maxCountSwitchToStickedForm)
            {
                TriggerTextBox();
                return;
            }
        }
    }
    #endregion
    #endregion
}
