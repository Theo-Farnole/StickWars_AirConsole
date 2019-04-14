using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TO Databases/Player Controller")]
public class PlayerControllerData : ScriptableObject
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
}
