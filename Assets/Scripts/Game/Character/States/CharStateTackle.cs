using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStateTackle : OwnerState<CharController>
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
        _owner.CharAudio.PlaySound(CharAudio.Sound.Tackle);

        if (_owner.Raycast.down)
        {
            _owner.CharFeedback.PlayOrientedParticle(true, CharFeedback.OrientateParticle.Tackle);
        }

        // update collider
        _owner.Collisions.SetCollider(CharController.CharacterCollisions.Collider.Transition);

        _owner.ExecuteAfterTime(TACKLE_TRANSITION_TIME, () =>
        {
            _owner.Collisions.SetCollider(CharController.CharacterCollisions.Collider.Tackle);
        });

        _owner.ExecuteAfterTime(TACKLE_DURATION, () =>
        {
            _owner.Collisions.SetCollider(CharController.CharacterCollisions.Collider.Normal);
            _owner.State = new CharStateNormal(_owner);
        });
    }

    public override void OnStateExit()
    {
        _owner.Collisions.SetCollider(CharController.CharacterCollisions.Collider.Normal);
        _owner.CharFeedback.PlayOrientedParticle(false, CharFeedback.OrientateParticle.Tackle);
        _owner.EntitiesHit.Clear();
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
        float direction = (int)_owner.OrientationX;
        Vector2 velocity = _owner.Rigidbody.velocity;

        if ((direction < 0 && _owner.Raycast.left == false) || (direction > 0 && _owner.Raycast.right == false))
        {
            velocity.x = _owner.Data.Speed * direction;
        }
        else
        {
            velocity.x = 0;
        }

        _owner.Rigidbody.velocity = velocity;
    }
    #endregion
}
