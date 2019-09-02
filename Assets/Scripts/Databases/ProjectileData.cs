using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Projectile")]
public class ProjectileData : ScriptableObject
{
    [SerializeField] private int _speed = 3;
    [SerializeField] private float _lifetimeOnCollision = 3;

    public int Speed { get => _speed; }
    public float LifetimeOnCollision { get => _lifetimeOnCollision; }
}
