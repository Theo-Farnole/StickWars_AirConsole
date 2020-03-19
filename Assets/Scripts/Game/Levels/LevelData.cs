using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelData : Singleton<LevelData>
{
    #region Fields
    [SerializeField] private Transform[] _virusSpawnerPosition = new Transform[3];
    [Space]
    [EnumNamedArray(typeof(CharId))]
    [SerializeField] private Transform[] _defaultSpawnPoint = new Transform[4];
    [SerializeField] private LineData[] _respawnArea;
    [SerializeField] private LineData[] _projectilePickupSpawnArea;
    #endregion

    #region Properties
    public Transform[] VirusSpawnerPosition { get => _virusSpawnerPosition;}
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void OnDrawGizmos()
    {
        // draw respawn area
        Gizmos.color = Color.red;
        for (int i = 0; i < _respawnArea.Length; i++)
            _respawnArea[i].DrawGizmos();

        // draw pickup area
        Gizmos.color = Color.yellow;
        foreach ( var pickupArea in _projectilePickupSpawnArea)
            pickupArea.DrawGizmos();
    }
    #endregion

    #region Getter
    #region Spawnpoints
    public Vector3 GetDefaultSpawnPoint(CharId charId)
    {
        return _defaultSpawnPoint[(int)charId].position;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        return _respawnArea.GetRandomPoint();        
    }
    #endregion

    #region VirusSpawner position
    public Vector3 GetRandomVirusSpawnerPosition()
    {
        int randomIndex = Random.Range(0, _virusSpawnerPosition.Length);

        return _virusSpawnerPosition[randomIndex].position;
    }

    public Vector3 GetRandomVirusSpawnerPosition(Vector3 dontInclude)
    {
        // remove dontInclude from list
        var result = _virusSpawnerPosition.Where((v, i) => v.position != dontInclude).ToArray();

        int randomIndex = Random.Range(0, result.Length);
        return result[randomIndex].position;
    }
    #endregion

    #region Projectile Pickup position
    public Vector3 GetRandomProjectilePickupPosition()
    {
        return _projectilePickupSpawnArea.GetRandomPoint();
    }
    #endregion
    #endregion
    #endregion
}
