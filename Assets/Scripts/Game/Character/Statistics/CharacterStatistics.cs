using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
// using CSharp If to avoid included DLL
using System.Text; // only used in ToString() override
#endif

public class CharacterStatistics
{
    private readonly CharId _charId;
    private readonly CharController _characterController;

    private int _jumpCount = 0;
    private int _virusReleased = 0;
    private int _tackleSumDamage = 0;
    private int _projectileThrowCount = 0;
    private int _projectilePickupPicked = 0;

    public CharacterStatistics(CharId charId, CharController characterController)
    {
        _charId = charId;
        _characterController = characterController;

        _characterController.OnJump += OnJump;
        _characterController.OnAttack += OnAttack;
        _characterController.OnAttackHit += OnAttackHit;
        _characterController.OnProjectilePickupPicked += OnProjectilePickupPicked;
        _characterController.GetComponent<Entity>().OnKillMade += OnKillMade;
    }

    #region Events handlers
    void OnJump(CharController jump)
    {
        _jumpCount++;
    }

    void OnKillMade(Entity killer, Entity victim)
    {
        if (victim is VirusSpawner)
        {
            int releasedVirusCount = GameManager.Instance.InstantiatedCharactersCount - 1;
            _virusReleased += releasedVirusCount;
        }
    }

    void OnAttack(CharController charController, CharacterAttackType characterAttackType)
    {
        if (characterAttackType == CharacterAttackType.Projectile)
        {
            _projectileThrowCount++;
        }
    }

    void OnAttackHit(CharController charController, CharacterAttackType characterAttackType, int damage)
    {
        if (characterAttackType == CharacterAttackType.Tackle)
        {
            _tackleSumDamage += damage;
        }
    }

    void OnProjectilePickupPicked(CharController charController)
    {
        _projectilePickupPicked++;
    }
    #endregion

#if UNITY_EDITOR
    public override string ToString()
    {
        StringBuilder o = new StringBuilder();

        o.AppendLine("Statistics of " + _charId);
        o.AppendLine("Jump count: " + _jumpCount);
        o.AppendLine("Virus released: " + _virusReleased);
        o.AppendLine("Tackle sum damage: " + _tackleSumDamage);
        o.AppendLine("Projectile throw count: " + _projectileThrowCount);
        o.AppendLine("Projectile pickup picked: " + _projectilePickupPicked);

        return o.ToString();
    }
#endif
}
