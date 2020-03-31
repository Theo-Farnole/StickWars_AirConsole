using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Event Controller")]
public class EventControllerData : ScriptableObject
{
    #region Fields
    [SerializeField, Range(0, 5)] private int _maxSpawnVirusSpawner = 3;
    #endregion

    #region Properties
    public int MaxSpawnVirusSpawner { get => _maxSpawnVirusSpawner;}
    #endregion
}

