using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePickupSpawner : MonoBehaviour
{
    #region Fields
    private const string POOL_ID_PROJECTILEPICKUP = "projectile_pickup";

    public ProjectilePickupDelegate OnProjectilePickupSpawn;

    [SerializeField] private ProjectilePickupSpawnerData _data;

    private float _timeToSpawnPickup = 0; // not a timer
    private float _killAmountToSpawnPickup = 0;

    private bool _disablePickupSpawning = false;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        UnityEngine.Assertions.Assert.IsNotNull(_data);

        ResetTimers();
    }

    void OnEnable()
    {
        GameManager.Instance.Gamemode.OnCharacterKill += OnCharacterKill;
        LevelLayoutManager.Instance.OnLevelLayoutAnimationStart += OnLevelLayoutAnimationStart;
        LevelLayoutManager.Instance.OnLevelLayoutAnimationEnd += OnLevelLayoutAnimationEnded;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Gamemode.OnCharacterKill -= OnCharacterKill;

        if (LevelLayoutManager.Instance != null)
        {
            LevelLayoutManager.Instance.OnLevelLayoutAnimationStart += OnLevelLayoutAnimationStart;
            LevelLayoutManager.Instance.OnLevelLayoutAnimationEnd += OnLevelLayoutAnimationEnded;
        }
    }

    void Update()
    {
        if (Time.time >= _timeToSpawnPickup)
        {            
            SpawnProjectilePickup();
        }
    }
    #endregion

    #region Events handler
    void OnCharacterKill(CharId killedCharacter)
    {
        int killAmount = GameManager.Instance.Gamemode.KillCount;

        if (_killAmountToSpawnPickup >= killAmount)
        {            
            SpawnProjectilePickup();
        }
    }

    void OnLevelLayoutAnimationStart(LevelLayoutManager levelLayoutManager)
    {
        _disablePickupSpawning = true;
    }

    void OnLevelLayoutAnimationEnded(LevelLayoutManager levelLayoutManager)
    {
        _disablePickupSpawning = false;
    }
    #endregion

    void SpawnProjectilePickup()
    {
        if (_disablePickupSpawning)
            return;

        ResetTimers();

        int projectilePickupInLevel = FindObjectsOfType<ProjectilePickup>().Length;

        // projectiles pickup in the level limit reached
        if (projectilePickupInLevel >= _data.MaxProjectilePickupsSimultaneously)
            return;

        var position = LevelDataLocator.GetLevelData().GetRandomProjectilePickupPosition();
        var rotation = Quaternion.identity;

        var projectilePickup = ObjectPooler.Instance.SpawnFromPool(POOL_ID_PROJECTILEPICKUP, position, rotation).GetComponent<ProjectilePickup>();

        OnProjectilePickupSpawn?.Invoke(projectilePickup);
    }

    private void ResetTimers()
    {
        _timeToSpawnPickup = Time.time + _data.TimeToSpawnPickup;
        _killAmountToSpawnPickup = GameManager.Instance.Gamemode.KillCount + _data.KillAmountToSpawnPickup;
    }
    #endregion
}
