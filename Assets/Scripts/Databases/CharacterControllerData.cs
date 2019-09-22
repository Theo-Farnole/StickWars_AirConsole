using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Character Controller")]
public class CharacterControllerData : ScriptableObject
{
    [Header("Movements")]

    [SerializeField] private float _speed = 3;
    public float Speed { get => _speed; }

    [SerializeField] private float _jumpForce = 500;
    public float JumpForce { get => _jumpForce; }

    [SerializeField] private float _slidingDownSpeed = 1.5f;
    public float SlidingDownSpeed { get => _slidingDownSpeed; }


    [Header("Attack")]

    [SerializeField] private int _damageTackle = 3;
    public int DamageTackle { get => _damageTackle; }

    [SerializeField] private int _damageProjectile = 2;
    public int DamageProjectile { get => _damageProjectile;  }

    [SerializeField] private float _cadenceProjectile = 2;
    public float CadenceProjectile { get => _cadenceProjectile; }

    [Header("Respawn")]

    [SerializeField] private float _respawnDuration = 1.5f;
    public float RespawnDuration { get => _respawnDuration; set => _respawnDuration = value; }
}
