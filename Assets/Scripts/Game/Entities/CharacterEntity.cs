using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharController))]
public class CharacterEntity : Entity
{
    #region Fields
    private int _myID = -1;
    private CharController _charController;
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
        _myID = (int)_charController.charID;
    }
    #endregion

    public override void GetDamage(int damage, Entity attacker)
    {
        if (attacker.GetComponent<VirusController>() && damage > _hp)
        {
            damage = _hp - 1;
        }

        base.GetDamage(damage, attacker);
    }

    protected override void Death(Entity killer)
    {
        // retrieve killer 
        var charController = killer.GetComponent<CharController>();
        CharId? killerId = null;

        if (charController != null)
        {
            killerId = killer.GetComponent<CharController>().charID;
        }

        GameManager.Instance.Gamemode.Kill(killerId);

        _charController.Respawn();
    }

    public void ResetHP()
    {
        _hp = MaxHp;
        UpdateHealthSlider();
    }
    #endregion
}
