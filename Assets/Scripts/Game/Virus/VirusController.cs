using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusController : MonoBehaviour
{
    #region Fields
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
        State = new VirusStateGoto(this);
    }

    void Update()
    {
        _state.Tick();    
    }

    void FixedUpdate()
    {
        _state.FixedTick();            
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, _data.AttackRange);
    }
    #endregion
}