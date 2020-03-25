using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractState_LevelLayoutManager : OwnerState<LevelLayoutManager>
{
    public AbstractState_LevelLayoutManager(LevelLayoutManager owner) : base(owner)
    {
    }

    // don't force to inherit fixed tick
    public override void FixedTick() { }

    // don't force to inherit tick
    public override void Tick() { }
}
