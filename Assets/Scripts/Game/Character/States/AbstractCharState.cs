using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCharState : OwnerState<CharController>
{
    public AbstractCharState(CharController owner) : base(owner)
    { }
}
