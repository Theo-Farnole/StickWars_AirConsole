using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharController))]
public class CharacterEntity : Entity
{
    #region Fields
    [Header("Character Entity configuration")]
    [Tooltip("Eg. If the character die in a deathzone, the last opponent who hit this character will be attributed the kill (if the last hit isn't old from _lastHitLifetimeToAttributeKill value).")]
    [SerializeField] private float _lastHitLifetimeToAttributeKill = 3;

    private int _myID = -1;
    private CharController _charController;

    private KeyValuePair<CharacterEntity, float> _lastCharacterAttacker = new KeyValuePair<CharacterEntity, float>(null, -1);
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    protected override void Awake()
    {
        base.Awake();

        _charController = GetComponent<CharController>();
    }

    void Start()
    {
        _myID = (int)_charController.charId;
    }
    #endregion

    public override void GetDamage(int damage, Entity attacker)
    {
        bool hasHitVirusController = attacker != null && attacker.GetComponent<VirusController>();
        if (hasHitVirusController)
        {
            // prevent VirusController to kill CharacterEntity
            if (damage >= _hp)
            {
                damage = _hp - 1;
            }
        }

        base.GetDamage(damage, attacker);

        if (attacker is CharacterEntity)
        {
            _lastCharacterAttacker = new KeyValuePair<CharacterEntity, float>(attacker as CharacterEntity, Time.time);
        }
    }

    protected override void Death(Entity killer)
    {
        CharId? killerId = null;

        // Retrieve killer
        if (killer != null && killer.GetComponent<CharController>())
        {
            killerId = killer.GetComponent<CharController>().charId;
        }
        else // has not been killed by a CharacterController.
        {
            // has been previously hit by a CharacterController ?
            if (_lastCharacterAttacker.Key != null && _lastCharacterAttacker.Value + _lastHitLifetimeToAttributeKill >= Time.time)
            {
                killerId = _lastCharacterAttacker.Key.GetComponent<CharController>().charId;
            }
            else
            {
                killerId = null;
            }
        }

        _charController.Respawn();
        GameManager.Instance.Gamemode.Kill(killerId);
    }

    public void ResetHP()
    {
        _hp = MaxHp;
        UpdateHealthSlider();
    }
    #endregion
}
