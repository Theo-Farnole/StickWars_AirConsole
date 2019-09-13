using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Event Controller")]
public class EventControllerData : ScriptableObject
{
    #region Fields
    [SerializeField, Range(0, 5)] private int _killsNeededToSpawnVirusSpawner = 1;
    #endregion

    #region Properties
    public int KillsNeededToSpawnVirusSpawner { get => _killsNeededToSpawnVirusSpawner;}
    #endregion
}

