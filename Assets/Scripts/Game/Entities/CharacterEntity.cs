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
        base.GetDamage(damage, attacker);

        //_charController.CharFeedback.PlayNonOrientedParticle(true, CharFeedback.Particle.Hitted);
    }

    protected override void Death(Entity killer)
    {
        // retrieve killer ID
        CharController killerCharController = killer.GetComponent<CharController>();
        int killerID = killerCharController ? (int)killerCharController.charID : -1;

        GameManager.Instance.Gamemode.Kill(killerID, _myID);

        _charController.Respawn();
    }

    public void ResetHP()
    {        
        _hp = MaxHp;
        UpdateHealthSlider();
    }
    #endregion
}
