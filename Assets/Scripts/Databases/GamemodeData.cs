using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TO/Databases/Gamemode")]
public class GamemodeData : ScriptableObject
{
    [SerializeField] private int _defaultValue = 3;

    public int DefaultValue { get => _defaultValue; }
}
