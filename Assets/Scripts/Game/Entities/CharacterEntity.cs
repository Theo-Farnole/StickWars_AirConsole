using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharController))]
public class CharacterEntity : Entity
{
    #region Fields
    private int _myID = -1;
    private CharController _charController;

    private bool _isDead = false;
    #endregion

    #region Properties
    public bool IsDead { get => _isDead; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _charController = GetComponent<CharController>();
    }

    protected override void Start()
    {
        base.Start();

        _myID = (int)_charController.charID;
    }
    #endregion

    protected override void Death(Entity killer)
    {
        _isDead = true;

        // retrieve killer ID
        CharController killerCharController = killer.GetComponent<CharController>();
        int killerID = killerCharController ? (int)killerCharController.charID : -1;

        GameManager.Instance.Gamemode.Kill(killerID, _myID);

        // feedback
        CameraShake.Instance.Shake();
        var deathPS = _charController.CharFeedback.GetNonOrientedParticle(CharFeedback.Particle.Death);
        Instantiate(deathPS, transform.position, Quaternion.identity).Play();

        // then respawn
        Respawn();
    }

    private void Respawn()
    {
        transform.position = LevelData.Instance.GetRandomSpawnPoint().position;

        _hp = MaxHp;
        UpdateHealthSlider();

        _charController.Respawn();
        _isDead = false;
    }
    #endregion
}
