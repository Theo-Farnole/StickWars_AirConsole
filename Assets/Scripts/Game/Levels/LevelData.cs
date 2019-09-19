using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : Singleton<LevelData>
{
    #region Classes
    [System.Serializable]
    public class RespawnArea
    {
        public Vector3 p1 = Vector3.left;
        public Vector3 p2 = Vector3.right;

        public void DrawGizmos()
        {
            Gizmos.DrawLine(p1, p2);
        }
    }
    #endregion

    #region Fields
    [SerializeField] private Transform[] _virusSpawnerPosition = new Transform[3];
    [Space]
    [EnumNamedArray(typeof(CharId))]
    [SerializeField] private Transform[] _defaultSpawnPoint = new Transform[4];
    [SerializeField] private RespawnArea[] _respawnArea;
    #endregion

    #region Properties
    public Transform[] VirusSpawnerPosition { get => _virusSpawnerPosition;}
    #endregion

    #region Methods
    public Vector3 GetDefaultSpawnPoint(CharId charId)
    {
        return _defaultSpawnPoint[(int)charId].position;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        int random = Random.Range(0, _respawnArea.Length);

        Vector3 p = Vector3.zero;
        p.x = Random.Range(_respawnArea[random].p1.x, _respawnArea[random].p2.x);
        p.y = Random.Range(_respawnArea[random].p1.y, _respawnArea[random].p2.y);
        p.z = Random.Range(_respawnArea[random].p1.y, _respawnArea[random].p2.z);

        return p;        
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < _respawnArea.Length; i++)
        {
            _respawnArea[i].DrawGizmos();
        }
    }
    #endregion
}
