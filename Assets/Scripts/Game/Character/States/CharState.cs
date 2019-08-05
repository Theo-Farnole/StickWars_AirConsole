using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharState : State
{
    protected CharController _charController;

    public abstract void FixedTick();

    public CharState(CharController charController) => _charController = charController;
}
