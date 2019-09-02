using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStateNormal : CharState
{
    private int _jumpCount = 0;

    public CharStateNormal(CharController charController) : base(charController)
    { }

    #region Methods
    #region State Transitions Callbacks
    public override void OnStateExit()
    {
        _charController.CharAudio.StopSound(CharAudio.Sound.Footstep);
    }
    #endregion
    #region Tick Callbacks
    public override void Tick()
    {
        ProcessAttackInputs();
        ManageStick();

        if (_charController.Raycast.down)
        {
            _jumpCount = 0;
        }
    }

    public override void FixedTick()
    {
        ProcessVerticalInput();
        ProcessHorizontalInput();
    }
    #endregion

    #region Ticks Methods
    #region Update
    void ProcessAttackInputs()
    {
        if (_charController.Inputs.throwPressed && _charController.CanThrowProjectile)
        {
            _charController.ThrowProjectile();
        }
        else if (_charController.Inputs.tacklePressed)
        {
            _charController.State = new CharStateTackle(_charController);
        }
    }

    void ManageStick()
    {
        if (_charController.Raycast.down == false &&
            ((_charController.Inputs.horizontalInput < 0 && _charController.Raycast.left) ||
            (_charController.Inputs.horizontalInput > 0 && _charController.Raycast.right)))
        {
            _charController.State = new CharStateSticked(_charController);
        }
    }
    #endregion

    #region Fixed Update
    void ProcessVerticalInput()
    {
        if (_charController.Inputs.jumpPressed && _jumpCount < CharController.MAX_JUMPS_COUNT)
        {
            _charController.Inputs.jumpPressed = false;
            Jump();
        }
    }

    void ProcessHorizontalInput()
    {
        float direction = _charController.Inputs.horizontalInput;
        Vector2 velocity = _charController.Rigidbody.velocity;

        if ((direction < 0 && _charController.Raycast.left == false) || (direction > 0 && _charController.Raycast.right == false))
        {
            velocity.x = _charController.Data.Speed * direction;
            _charController.CharAudio.PlaySound(CharAudio.Sound.Footstep);
        }
        else
        {
            velocity.x = 0;
            _charController.CharAudio.StopSound(CharAudio.Sound.Footstep);
        }

        _charController.Rigidbody.velocity = velocity;

        if (direction != 0)
        {
            _charController.OrientationX = direction < 0 ? CharController.Orientation.Left : CharController.Orientation.Right;
        }
    }

    void Jump()
    {
        _charController.CharAudio.PlaySound(CharAudio.Sound.Jump);
        _charController.CharFeedback.PlayNonOrientedParticle(true, CharFeedback.Particle.Jump);
        _jumpCount++;

        _charController.Rigidbody.velocity = new Vector2(_charController.Rigidbody.velocity.x, 0);
        _charController.Rigidbody.AddForce(Vector2.up * _charController.Data.JumpForce);
    }
    #endregion
    #endregion
    #endregion
}
