using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Projectile Pickup Spawner")]
public class ProjectilePickupSpawnerData : ScriptableObject
{
    [Tooltip("Inclusive; Eg. if this variable is set at 3, there'll only be 3 projectile pickups simultaneously in the level maximum.")]
    [SerializeField] private int _maxProjectilePickupsSimultaneously = 1;
    [Space]
    [Tooltip("In seconds")]
    [SerializeField] private float _timeToSpawnPickup = 10;
    [SerializeField] private float _killAmountToSpawnPickup = 2;

    public int MaxProjectilePickupsSimultaneously { get => _maxProjectilePickupsSimultaneously; }
    public float TimeToSpawnPickup { get => _timeToSpawnPickup; }
    public float KillAmountToSpawnPickup { get => _killAmountToSpawnPickup; }
}
