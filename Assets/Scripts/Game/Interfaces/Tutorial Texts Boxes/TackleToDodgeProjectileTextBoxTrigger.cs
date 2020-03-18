using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TackleToDodgeProjectileTextBoxTrigger : AbstractTextBoxTrigger
{
    [Header("Condition")]
    [SerializeField] private int _projectilesHitCountToTrigger = 8;

    private Dictionary<CharacterEntity, int> _projectilesHitCount = new Dictionary<CharacterEntity, int>();

    void OnEnable()
    {
        Projectile.OnProjectileHitEntity += OnProjectileHitEntity;
    }

    void OnDisable()
    {
        Projectile.OnProjectileHitEntity -= OnProjectileHitEntity;
    }

    void OnProjectileHitEntity(Entity ent)
    {
        if (!(ent is CharacterEntity))
            return;

        var key = ent as CharacterEntity;

        // create new entry in dictionary
        if (!_projectilesHitCount.ContainsKey(key))
            _projectilesHitCount.Add(key, 0);

        // increase hit count
        _projectilesHitCount[key]++;
        
        if (_projectilesHitCount[key] == _projectilesHitCountToTrigger)
        {
            TriggerTextBox();
        }
    }
}
