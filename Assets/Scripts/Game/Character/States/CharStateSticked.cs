using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStateSticked : CharState
{
    public CharStateSticked(CharController charController) : base(charController)
    { }

    #region Methods
    #region Callbacks
    #region State Transition Callbacks
    public override void OnStateEnter()
    {
        _charController.Rigidbody.gravityScale = 0;
        _charController.CharFeedback.PlayOrientedParticle(true, CharFeedback.OrientateParticle.SlidingWall);
    }

    public override void OnStateExit()
    {
        _charController.Rigidbody.gravityScale = 1;
        _charController.CharFeedback.PlayOrientedParticle(false, CharFeedback.OrientateParticle.SlidingWall);
    }
    #endregion

    #region Tick Callbacks
    public override void Tick()
    {
        if (_charController.Raycast.down ||
            _charController.Inputs.horizontalInput == 0 ||
            (_charController.Inputs.horizontalInput < 0 && !_charController.Raycast.left) ||
            (_charController.Inputs.horizontalInput > 0 && !_charController.Raycast.right))
        {
            _charController.State = new CharStateNormal(_charController);
        }
    }

    public override void FixedTick()
    {
        // slow sliding
        Vector2 velocity = _charController.Rigidbody.velocity;

        velocity.y = _charController.Data.SlidingDownSpeed;

        _charController.Rigidbody.velocity = velocity;
    }
    #endregion
    #endregion
    #endregion
}
