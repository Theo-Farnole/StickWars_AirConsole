using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : Singleton<EventController>
{
    private const KeyCode DEBUG_KEY_SPAWNVIRUSSPAWNER = KeyCode.P;
    #region Fields
    public VirusSpawnerDelegate OnVirusSpawnerSpawned;

    [SerializeField] private EventControllerData _data;
    [Space]
    [SerializeField] private GameObject _prefabVirusSpawner;
    [SerializeField] private Transform _positionVirusSpawner;

    private GameObject _currentVirusSpawner = null;
    #endregion

    #region Methods
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKey(DEBUG_KEY_SPAWNVIRUSSPAWNER))
            InstantiateVirusSpawner();
    }
#endif

    public void OnKill()
    {
        // calculating sum kills goal
        int killsGoalPerPlayer = GameManager.Instance.Gamemode.ValueForVictory;
        int playersCount = GameManager.Instance.InstantiatedCharactersCount;
        int sumKillsGoal = killsGoalPerPlayer * playersCount; 

        // calcuting multiple to spawn virus spawner
        int multiple = (sumKillsGoal - 2 * playersCount) / _data.MaxSpawnVirusSpawner;

        Debug.LogFormat("Multiple is {0}; value is {1}", multiple, GameManager.Instance.Gamemode.ValueForVictory);

        // check if kill number is a multiple of multiple
        int killNumber = GameManager.Instance.Gamemode.SumCharactersValue;
        bool shouldSpawnVirus = (killNumber % multiple == 0);
          
        // spawn virus spawner if there isn't VirusController in the map
        if (shouldSpawnVirus)
        {
            InstantiateVirusSpawner();
        }
    }

    private void InstantiateVirusSpawner()
    {
        // prevent spawning another virus spawner
        if (_currentVirusSpawner != null)
            return;
        
        // prevent spawning if viruses are on the map
        if (FindObjectsOfType<VirusController>().Length != 0)
            return;
        
        _currentVirusSpawner = Instantiate(_prefabVirusSpawner, _positionVirusSpawner.position, Quaternion.identity);

        OnVirusSpawnerSpawned?.Invoke(_currentVirusSpawner.GetComponent<VirusSpawner>());
    }
    #endregion
}
