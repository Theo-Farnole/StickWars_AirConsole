using System.Collections;
using System.Collections.Generic;
using TF.Utilities.RemoteConfig;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Projectile")]
public class ProjectileData : RemoteConfigScriptableObject
{
    [SerializeField] private int _speed = 3;
    [SerializeField] private float _lifetimeOnCollision = 3;

    public int Speed { get => _speed; }
    public float LifetimeOnCollision { get => _lifetimeOnCollision; }
}
