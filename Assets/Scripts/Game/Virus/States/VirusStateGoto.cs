using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusStateGoto : OwnerState<VirusController>
{
    #region Methods
    public VirusStateGoto(VirusController owner) : base(owner) { }

    public override void FixedTick() { }
    public override void Tick()
    {
        _owner.transform.position = Vector3.MoveTowards(_owner.transform.position, _owner.target.position, _owner.Data.Speed * Time.deltaTime);
    }
    #endregion
}
