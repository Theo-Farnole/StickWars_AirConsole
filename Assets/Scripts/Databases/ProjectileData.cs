using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Projectile")]
public class ProjectileData : ScriptableObject
{
    [SerializeField] private int _speed = 3;

    public int Speed { get => _speed; }
}
