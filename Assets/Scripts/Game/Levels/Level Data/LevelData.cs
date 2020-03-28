using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    #region Fields
    [SerializeField] private int _activeOnLayout;
    [Header("PLAYERS SPAWN / RESPAWN")]
    [SerializeField, EnumNamedArray(typeof(CharId))] private Transform[] _defaultSpawnPoint = new Transform[4];
    [SerializeField] private LineData[] _respawnArea;
    [Header("COLLECTABLES SPAWN")]
    [SerializeField] private Transform[] _virusSpawnerPosition = new Transform[3];
    [SerializeField] private LineData[] _projectilePickupSpawnArea;

    private bool gizmosEnabled = true;
    #endregion

    #region Properties
    public Transform[] VirusSpawnerPosition { get => _virusSpawnerPosition; }
    public int ActiveOnLayout { get => _activeOnLayout; }
    public bool GizmosEnabled { get => gizmosEnabled; set => gizmosEnabled = value; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void OnDrawGizmos()
    {
        if (!GizmosEnabled)
            return;

        const float gizmosSphereRadius = 0.1f;

        // draw default spawn point
        Gizmos.color = Color.black;
        foreach (var defaultSpawn in _defaultSpawnPoint)
        {
            if (defaultSpawn != null)
                Gizmos.DrawSphere(defaultSpawn.position, gizmosSphereRadius);
        }

        // draw respawn area
        Gizmos.color = Color.green;
        for (int i = 0; i < _respawnArea.Length; i++)
            _respawnArea[i].DrawGizmos();

        // draw virus spawner position
        Gizmos.color = Color.red;
        foreach (var virusSpawner in _virusSpawnerPosition)
        {
            if (virusSpawner != null)
                Gizmos.DrawSphere(virusSpawner.position, gizmosSphereRadius);
        }

        // draw pickup area
        Gizmos.color = Color.yellow;
        foreach (var pickupArea in _projectilePickupSpawnArea)
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
