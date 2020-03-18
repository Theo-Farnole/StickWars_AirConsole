using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCharState : OwnerState<CharController>
{
    // TODO: Inherit others CharState from this class

    // TODO: OnStateChanged events in CharController
    public AbstractCharState(CharController owner) : base(owner)
    { }
}
