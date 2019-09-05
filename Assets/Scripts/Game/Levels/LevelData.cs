using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : Singleton<LevelData>
{
    #region Fields
    public static readonly float SPAWN_REUSE_TIME = 1f;

    [SerializeField] private Transform[] _spawnPoints = new Transform[4];
    [SerializeField] private Transform[] _virusTriggererPosition = new Transform[3];

    private List<Transform> _availableSpawnPoints = null;
    #endregion

    #region Properties
    public Transform[] VirusTriggererPosition { get => _virusTriggererPosition;}
    #endregion

    #region Methods
    public Transform GetRandomSpawnPoint()
    {
        // init spawn points
        if (_availableSpawnPoints == null)
        {
            _availableSpawnPoints = new List<Transform>(_spawnPoints);
        }

        Transform point;

        if (_availableSpawnPoints != null)
        {
            int randomIndex = Random.Range(0, _availableSpawnPoints.Count);
            point = _availableSpawnPoints[randomIndex];

            // avoid same spawn on multiple players
            _availableSpawnPoints.Remove(point);

            this.ExecuteAfterTime(SPAWN_REUSE_TIME, () =>
            {
                _availableSpawnPoints.Add(point);
            });
        }

        // if not available spawn point exist,
        // take random one in original array
        else
        {
            Debug.Log("Not available spawn point, taking one random");

            int randomIndex = Random.Range(0, _spawnPoints.Length);
            point = _spawnPoints[randomIndex];
        }

        return point;
    }
    #endregion
}
