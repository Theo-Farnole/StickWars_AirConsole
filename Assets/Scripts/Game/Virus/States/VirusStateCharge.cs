using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusStateCharge : OwnerState<VirusController>
{
    #region Fields
    private Vector3 _targetPosition;
    #endregion

    #region Methods
    public VirusStateCharge(VirusController owner) : base(owner) { }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        _targetPosition = _owner.transform.position + _owner.transform.up * _owner.Data.ChargeDistance;
    }

    public override void FixedTick() { }
    public override void Tick()
    {
        Transform ownerTrf = _owner.transform;
        var data = _owner.Data;

        ownerTrf.position = Vector3.MoveTowards(ownerTrf.position, _targetPosition, data.ChargeSpeed * Time.deltaTime);

        if (Vector3.Distance(ownerTrf.position, _targetPosition) <= VirusController.DISTANCE_THRESHOLD)
        {
            _owner.State = new VirusStateGoto(_owner);
        }
    }
    #endregion
}
