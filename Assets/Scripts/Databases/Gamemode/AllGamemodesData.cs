using System;
using System.Collections;
using System.Collections.Generic;
using TF.Utilities.RemoteConfig;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Gamemode/AllGamemodes")]
public class AllGamemodesData : RemoteConfigScriptableObject
{
    [EnumNamedArray(typeof(GamemodeType))]
    [SerializeField] private GamemodeData[] _gamemodeData = new GamemodeData[Enum.GetValues(typeof(GamemodeType)).Length];

    public int GetGamemodeValue(GamemodeType gamemodeType)
    {
        return _gamemodeData[(int)gamemodeType].DefaultValue;
    }

}
