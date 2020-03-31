using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="StickWars/Level/Cursor")]
public class CursorData : ScriptableObject
{
    [SerializeField] private float _speed = 3;

    public float Speed { get => _speed; }
}
