using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStateSticked : AbstractCharState
{
    public CharStateSticked(CharController charController) : base(charController)
    { }

    #region Methods
    #region Callbacks
    #region State Transition Callbacks
    public override void OnStateEnter()
    {
        _owner.Rigidbody.gravityScale = 0;
        _owner.CharFeedback.PlayOrientedParticle(true, CharFeedback.OrientateParticle.SlidingWall);
    }

    public override void OnStateExit()
    {
        _owner.Rigidbody.gravityScale = 1;
        _owner.CharFeedback.PlayOrientedParticle(false, CharFeedback.OrientateParticle.SlidingWall);
    }
    #endregion

    #region Tick Callbacks
    public override void Tick()
    {
        if (_owner.Raycast.down ||
            _owner.Inputs.HorizontalInput == 0 ||
            (_owner.Inputs.HorizontalInput < 0 && !_owner.Raycast.left) ||
            (_owner.Inputs.HorizontalInput > 0 && !_owner.Raycast.right))
        {
            _owner.State = new CharStateNormal(_owner);
        }
    }

    public override void FixedTick()
    {
        // slow sliding
        Vector2 velocity = _owner.Rigidbody.velocity;

        velocity.y = _owner.Data.SlidingDownSpeed;

        _owner.Rigidbody.velocity = velocity;
    }
    #endregion
    #endregion
    #endregion
}
