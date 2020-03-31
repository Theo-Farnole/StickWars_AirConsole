using System.Collections;
using System.Collections.Generic;
using TF.Utilities.RemoteConfig;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Gamemode/Gamemode")]
public class GamemodeData : RemoteConfigScriptableObject
{
    [SerializeField] private int _defaultValue = 3;

    public int DefaultValue { get => _defaultValue; }
}
