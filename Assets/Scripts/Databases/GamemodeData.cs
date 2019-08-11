using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TO/Databases/Gamemode")]
public class GamemodeData : ScriptableObject
{
    [SerializeField] private int[] _valuesSettings = new int[5];

    public int[] ValuesSettings { get => _valuesSettings; }
}
