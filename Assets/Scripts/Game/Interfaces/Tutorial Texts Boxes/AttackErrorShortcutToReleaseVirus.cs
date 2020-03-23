using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackErrorShortcutToReleaseVirus : AbstractTextBoxTrigger
{
    #region Fields
    [Header("Condition")]
    [SerializeField] private float _timeToTriggerTextBox = 10;

    private int _virusSpawnerHitsCount = 0;
    private VirusSpawner _currentVirusSpawner;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        EventController.Instance.OnVirusSpawnerSpawned += OnVirusSpawnerSpawned;
    }

    void OnDisable()
    {
        if (EventController.Instance != null)
            EventController.Instance.OnVirusSpawnerSpawned -= OnVirusSpawnerSpawned;

        if (_currentVirusSpawner != null)
            _currentVirusSpawner.OnDamage.RemoveListener(OnDamage);
    }
    #endregion

    #region Events listeners
    void OnVirusSpawnerSpawned(VirusSpawner virusSpawner)
    {
        Debug.Log("VirusSpawner spawned");

        _currentVirusSpawner = virusSpawner;

        // start timer to check if virusController has been hit
        StartTimer(_timeToTriggerTextBox, CheckConditionToTriggerTextBox);

        // record virus spawner's hits count
    
    }

    void OnDamage(Entity victim, int damageAmount)
    {
        Debug.Log("OnDamage virus");
        _virusSpawnerHitsCount++;
    }
    #endregion

    #region Checking methods
    void CheckConditionToTriggerTextBox()
    {
        Debug.Log("Timer ended");

        // We could directly use a boolean to test if virus spawn has been hit.
        // However, if later we want to display the text after 2 or more hit, we can do it easily
        bool virusHasBeenHit = (_virusSpawnerHitsCount > 0);

        if (!virusHasBeenHit)
        {
            TriggerTextBox();
        }
    }
    #endregion
    #endregion
}
