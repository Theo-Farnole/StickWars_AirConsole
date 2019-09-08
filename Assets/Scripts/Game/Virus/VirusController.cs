using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusController : MonoBehaviour
{
    #region Fields
    public static readonly float DISTANCE_THRESHOLD = 0.1f;

    [HideInInspector] public Transform target;

    [SerializeField] private VirusControllerData _data;
    [SerializeField] private GameObject _prefabDestroyParticleSystem;

    private OwnerState<VirusController> _state;
    private bool _isApplicationQuitting = false;
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
    void Awake()
    {
        GetComponent<Entity>().isInvincible = true;    
    }

    void Start()
    {
        this.ExecuteAfterTime(_data.DelayAfterTriggered, () =>
        {
            GetComponent<Entity>().isInvincible = false;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = GetComponent<Entity>();
        collision.GetComponent<CharacterEntity>()?.GetDamage(_data.AttackDamage, entity);
    }

    void OnDestroy()
    {
        if (!_isApplicationQuitting)
        {
            Instantiate(_prefabDestroyParticleSystem, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().Play();
        }
    }

    void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
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