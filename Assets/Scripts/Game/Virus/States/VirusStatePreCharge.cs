using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusStatePreCharge : OwnerState<VirusController>
{
    #region Fields
    private Vector3 _startingPosition;
    #endregion

    #region Methods
    public VirusStatePreCharge(VirusController owner) : base(owner) { }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        _owner.ResetHitCharacterEntity();
        _startingPosition = _owner.transform.position - _owner.transform.up * _owner.Data.PreChargeDistance;
    }

    public override void OnStateExit()
    {
        base.OnStateExit();

        _owner.ResetHitCharacterEntity();
    }

    public override void FixedTick() { }
    public override void Tick()
    {
        Transform ownerTrf = _owner.transform;
        var data = _owner.Data;

        ownerTrf.position = Vector3.MoveTowards(ownerTrf.position, _startingPosition, data.PreChargeSpeed * Time.deltaTime);

        if (Vector3.Distance(ownerTrf.position, _startingPosition) <= VirusController.DISTANCE_THRESHOLD)
        {
            _owner.State = new VirusStateCharge(_owner);
        }
    }
    #endregion
}
