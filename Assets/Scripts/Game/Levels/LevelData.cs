using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : Singleton<LevelData>
{
    public static readonly float SPAWN_REUSE_TIME = 1f;

    [SerializeField] private Transform[] _spawnPoints = new Transform[4];
    private List<Transform> _availableSpawnPoints = new List<Transform>();

    void Awake()
    {
        _availableSpawnPoints = new List<Transform>(_spawnPoints);
    }

    public Transform GetRandomSpawnPoint()
    {
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
            int randomIndex = Random.Range(0, _spawnPoints.Length);
            point = _spawnPoints[randomIndex];
        }

        return point;
    }
}
