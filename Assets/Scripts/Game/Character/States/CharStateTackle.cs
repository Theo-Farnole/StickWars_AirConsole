using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStateTackle : CharState
{
    #region Fields
    public readonly static float TACKLE_TRANSITION_TIME = 0.1f;
    public readonly static float TACKLE_DURATION = 1.1f / 2f;
    #endregion

    public CharStateTackle(CharController charController) : base(charController)
    { }

    #region Methods
    #region Callbacks
    #region State Transitions Callbacks
    public override void OnStateEnter()
    {
        _charController.CharAudio.PlaySound(CharAudio.Sound.Tackle);

        if (_charController.Raycast.down)
        {
            _charController.CharFeedback.PlayParticle(true, CharFeedback.Particle.Tackle);
        }

        // update collider
        _charController.Collisions.SetCollider(CharController.CharacterCollisions.Collider.Transition);

        _charController.ExecuteAfterTime(TACKLE_TRANSITION_TIME, () =>
        {
            _charController.Collisions.SetCollider(CharController.CharacterCollisions.Collider.Tackle);
        });

        _charController.ExecuteAfterTime(TACKLE_DURATION, () =>
        {
            _charController.Collisions.SetCollider(CharController.CharacterCollisions.Collider.Normal);
            _charController.State = new CharStateNormal(_charController);
        });
    }

    public override void OnStateExit()
    {
        _charController.Collisions.SetCollider(CharController.CharacterCollisions.Collider.Normal);
        _charController.CharFeedback.PlayParticle(false, CharFeedback.Particle.Tackle);
        _charController.EntitiesHit.Clear();
    }
    #endregion

    #region Ticks Callbacks
    public override void Tick() { }

    public override void FixedTick()
    {
        MoveCharacter();
    }
    #endregion
    #endregion

    void MoveCharacter()
    {
        float direction = (int)_charController.OrientationX;
        Vector2 velocity = _charController.Rigidbody.velocity;

        if ((direction < 0 && _charController.Raycast.left == false) || (direction > 0 && _charController.Raycast.right == false))
        {
            velocity.x = _charController.Data.Speed * direction;
        }
        else
        {
            velocity.x = 0;
        }

        _charController.Rigidbody.velocity = velocity;
    }
    #endregion
}
