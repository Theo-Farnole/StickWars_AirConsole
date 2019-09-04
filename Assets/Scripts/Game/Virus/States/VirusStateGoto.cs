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
        Transform ownerTrf = _owner.transform;
        Transform targetTrf = _owner.target.transform;
        var data = _owner.Data;

        //float angle = Vector3.Angle(ownerTrf.position, targetTrf.position);
        //ownerTrf.eulerAngles = ownerTrf.eulerAngles.SetZ(angle);
        ownerTrf.up = targetTrf.position - ownerTrf.position;
        ownerTrf.position = Vector3.MoveTowards(ownerTrf.position, targetTrf.position, data.Speed * Time.deltaTime);

        if (Vector3.Distance(ownerTrf.position, targetTrf.position) <= data.AttackRange)
        {
            _owner.State = new VirusStatePreCharge(_owner);
        }
    }
    #endregion
}
