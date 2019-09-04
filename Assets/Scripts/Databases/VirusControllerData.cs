using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Virus Controller")]
public class VirusControllerData : ScriptableObject
{
    [SerializeField, Tooltip("Unit per second")] private float _speed = 3;
    [SerializeField, Tooltip("In unit")] private float _attackRange = 3;

    public float Speed { get => _speed; }
    public float AttackRange { get => _attackRange; }
}
