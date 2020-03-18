using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Virus Controller")]
public class VirusControllerData : ScriptableObject
{
    #region Fields
    [Header("Global Config")]
    [SerializeField, Tooltip("Unit per second")] private float _speed = 3;
    [SerializeField] private float _delayAfterTriggered = 0.5f;

    [Header("Attack Config")]
    [Header("Attack Config > Movement")]
    [SerializeField] private float _attackRange = 3;
    [Space]
    [SerializeField] private float _preChargeDistance = 0.3f;
    [SerializeField] private float _preChargeTime = 0.8f;
    [Space]
    [SerializeField] private float _chargeTime = 0.3f;    
    [Header("Attack Config > Damage")]
    [SerializeField] private int _attackDamage = 3;
    [SerializeField] private float _delayBetweenAttackInflict = 0.8f;
    #endregion

    #region Properties
    public float Speed { get => _speed; }
    public float DelayAfterTriggered { get => _delayAfterTriggered; }

    public float AttackRange { get => _attackRange; }

    public float PreChargeDistance { get => _preChargeDistance; }
    public float PreChargeTime { get => _preChargeTime; }
    public float PreChargeSpeed { get => _preChargeDistance / _preChargeTime; }

    public float ChargeDistance { get => _attackRange; }
    public float ChargeTime { get => _chargeTime; }
    public float ChargeSpeed { get => ChargeDistance / _chargeTime; }

    public int AttackDamage { get => _attackDamage; }
    public float DelayBetweenAttackInflict { get => _delayBetweenAttackInflict; }
    #endregion
}
