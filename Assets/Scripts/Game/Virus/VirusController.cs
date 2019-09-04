using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusController : MonoBehaviour
{
    #region Fields
    public static readonly float DISTANCE_THRESHOLD = 0.1f;

    [HideInInspector] public Transform target;

    [SerializeField] private VirusControllerData _data;

    private OwnerState<VirusController> _state;
    #endregion

    #region Properties
    public OwnerState<VirusController> State
    {
        get
        {
            return _state;
        }

        set
        {
            if (_state != null)
                _state.OnStateExit();

            _state = value;

            if (_state != null)
                _state.OnStateEnter();
        }
    }

    public VirusControllerData Data { get => _data; }
    #endregion

    #region Methods
    void Start()
    {
        this.ExecuteAfterTime(_data.DelayAfterTriggered, () =>
        {
            State = new VirusStateGoto(this);
        });
    }

    void Update()
    {
        _state?.Tick();
    }

    void FixedUpdate()
    {
        _state?.FixedTick();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _data.AttackRange);

        if (_state is VirusStatePreCharge)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position - transform.forward * _data.PreChargeDistance, 0.1f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + transform.forward * _data.ChargeDistance, 0.1f);
        }
    }
    #endregion
}