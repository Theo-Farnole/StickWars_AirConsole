using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Gamemode/Gamemode")]
public class GamemodeData : ScriptableObject
{
    [SerializeField] private int _defaultValue = 3;

    public int DefaultValue { get => _defaultValue; }
}
