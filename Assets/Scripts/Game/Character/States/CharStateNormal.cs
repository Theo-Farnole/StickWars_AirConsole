﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStateNormal : AbstractCharState
{
    private int _jumpCount = 0;

    public CharStateNormal(CharController charController) : base(charController)
    { }

    #region Methods
    #region State Transitions Callbacks
    public override void OnStateExit()
    {
        _owner.CharAudio.StopSound(CharAudio.Sound.Footstep);
    }
    #endregion

    #region Tick Callbacks
    public override void Tick()
    {
        ProcessAttackInputs();
        ManageStick();

        if (_owner.Raycast.down)
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
        // c'est très sale :/
        if (_owner.Inputs.ThrowDown && !_owner.HasEnoughtCarriedProjectileToThrow)
        {
            _owner.NoCarriedProjectileOnThrow?.Invoke();
        }

        if (_owner.Inputs.ThrowPressed)
        {
            _owner.ThrowProjectile();
        }
        else if (_owner.Inputs.TacklePressed)
        {
            _owner.State = new CharStateTackle(_owner);
        }
    }

    void ManageStick()
    {
        if (_owner.Raycast.down == false &&
            ((_owner.Inputs.HorizontalInput < 0 && _owner.Raycast.left) ||
            (_owner.Inputs.HorizontalInput > 0 && _owner.Raycast.right)))
        {
            _owner.State = new CharStateSticked(_owner);
        }
    }
    #endregion

    #region Fixed Update
    void ProcessVerticalInput()
    {
        if (_owner.Inputs.JumpPressed && _jumpCount < CharController.MAX_JUMPS_COUNT)
        {
            _owner.Inputs.JumpPressed = false;
            Jump();
        }
    }

    void ProcessHorizontalInput()
    {
        float direction = _owner.Inputs.HorizontalInput;
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

        if (direction != 0)
        {
            _owner.OrientationX = direction < 0 ? CharController.Orientation.Left : CharController.Orientation.Right;
        }

        ManageFootstepSound(velocity);
    }

    void Jump()
    {
        _owner.CharAudio.PlaySound(CharAudio.Sound.Jump);
        _owner.CharFeedback.PlayNonOrientedParticle(true, CharFeedback.Particle.Jump);
        _jumpCount++;

        _owner.Rigidbody.velocity = new Vector2(_owner.Rigidbody.velocity.x, 0);
        _owner.Rigidbody.AddForce(Vector2.up * _owner.Data.JumpForce);

        _owner.OnJump?.Invoke(_owner);

        // on double jump
        if (_jumpCount > 1)
            _owner.OnDoubleJump?.Invoke(_owner);
    }

    void ManageFootstepSound(Vector2 velocity)
    {
        if (_owner.Raycast.down == false)
        {
            _owner.CharAudio.StopSound(CharAudio.Sound.Footstep);
        }
        else
        {
            if (velocity.x == 0)
            {
                _owner.CharAudio.StopSound(CharAudio.Sound.Footstep);
            }
            else
            {
                _owner.CharAudio.PlaySound(CharAudio.Sound.Footstep);
            }
        }
    }
    #endregion
    #endregion
    #endregion
}
