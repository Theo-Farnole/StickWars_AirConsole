using System.Collections;
using System.Collections.Generic;
using TF.Utilities.RemoteConfig;
using UnityEngine;

[CreateAssetMenu(menuName ="StickWars/Level/Cursor")]
public class CursorData : RemoteConfigScriptableObject
{
    [SerializeField] private float _speed = 3;

    public float Speed { get => _speed; }
}
