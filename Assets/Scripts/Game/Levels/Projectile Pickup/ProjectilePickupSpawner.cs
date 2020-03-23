using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePickupSpawner : MonoBehaviour
{
    #region Fields
    private const string TAG_PROJECTILEPICKUP = "projectile_pickup";

    public ProjectilePickupDelegate OnProjectilePickupSpawn;

    [Tooltip("Inclusive; Eg. if this variable is set at 3, there'll only be 3 projectile pickups simultaneously in the level maximum.")]
    [SerializeField] private int _maxProjectilePickupsSimultaneously = 1;
    [Space]
    [Tooltip("In seconds")]
    [SerializeField] private float _timeToSpawnPickup = 10;
    [SerializeField] private float _killAmountToSpawnPickup = 2;

    private float _currentTimeToSpawnPickup = 0;
    private float _currentKillAmountToSpawnPickup = 0;
    #endregion


    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        _currentTimeToSpawnPickup = Time.time + _timeToSpawnPickup;
        _currentKillAmountToSpawnPickup += _killAmountToSpawnPickup;
    }

    void OnEnable()
    {
        GameManager.Instance.Gamemode.OnCharacterKill += OnCharacterKill;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Gamemode.OnCharacterKill -= OnCharacterKill;
    }

    void Update()
    {
        if (Time.time >= _currentTimeToSpawnPickup)
        {
            _currentTimeToSpawnPickup = Time.time + _timeToSpawnPickup;
            SpawnProjectilePickup();
        }
    }
    #endregion

    #region Events handler
    void OnCharacterKill(CharId killedCharacter)
    {
        int killAmount = GameManager.Instance.Gamemode.KillCount;

        if (_currentKillAmountToSpawnPickup >= killAmount)
        {
            _currentKillAmountToSpawnPickup += _killAmountToSpawnPickup;
            SpawnProjectilePickup();
        }
    }
    #endregion

    void SpawnProjectilePickup()
    {
        int projectilePickupInLevel = FindObjectsOfType<ProjectilePickup>().Length;

        // there is 
        if (projectilePickupInLevel >= _maxProjectilePickupsSimultaneously)
            return;

        var position = LevelData.Instance.GetRandomProjectilePickupPosition();
        var rotation = Quaternion.identity;

        var projectilePickup = ObjectPooler.Instance.SpawnFromPool(TAG_PROJECTILEPICKUP, position, rotation).GetComponent<ProjectilePickup>();

        OnProjectilePickupSpawn?.Invoke(projectilePickup);
    }
    #endregion
}
