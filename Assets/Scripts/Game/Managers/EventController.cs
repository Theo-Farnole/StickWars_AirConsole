using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : Singleton<EventController>
{
    #region Fields
    [SerializeField] private EventControllerData _data;
    [Space]
    [SerializeField] private GameObject _prefabVirusSpawner;
    [SerializeField] private Transform _positionVirusSpawner;

    private GameObject _currentVirusSpawner = null;
    #endregion

    #region Methods
    public void OnKill()
    {
        var killNumber = GameManager.Instance.Gamemode.SumCharactersValue;

        bool shouldSpawnVirus = (_data.KillsNeededToSpawnVirusSpawner != 0 && killNumber % _data.KillsNeededToSpawnVirusSpawner == 0);

        if (shouldSpawnVirus && _currentVirusSpawner == null)
        {
            Debug.Log("Creating CurrentVirusSpawner at " + killNumber + " kills.");
            _currentVirusSpawner = Instantiate(_prefabVirusSpawner, _positionVirusSpawner.position, Quaternion.identity);
        }
    }
    #endregion
}
