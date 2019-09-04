using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusStateAttack : OwnerState<VirusController>
{
    #region Methods
    public VirusStateAttack(VirusController owner) : base(owner) { }

    public override void FixedTick() { }
    public override void Tick() { }
    #endregion
}
