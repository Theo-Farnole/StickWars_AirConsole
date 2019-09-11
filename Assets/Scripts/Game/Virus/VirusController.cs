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
    [Space]
    [SerializeField] private AudioSource _audioDeath;
    [SerializeField] private AudioSource _audioDoDamage;

    private OwnerState<VirusController> _state;
    private bool _isApplicationQuitting = false;

    private Entity _entity;
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
        _entity = GetComponent<Entity>();

        _entity.isInvincible = true;
    }

    void Start()
    {
        this.ExecuteAfterTime(_data.DelayAfterTriggered, () =>
        {
            _entity.isInvincible = false;
            State = new VirusStateGoto(this);
        });

        DynamicsObjects.Instance.SetToParent(transform, "virus");
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
        var characterEntity = collision.GetComponent<CharacterEntity>();

        if (characterEntity != null)
        {
            characterEntity.GetDamage(_data.AttackDamage, _entity);
            _audioDoDamage.Stop();
            _audioDoDamage.Play();
        }
    }

    // not a mistake: virus do damage OnEnter & OnExit
    void OnTriggerExit2D(Collider2D collision)
    {
        collision.GetComponent<CharacterEntity>()?.GetDamage(_data.AttackDamage, _entity);
    }

    void OnDestroy()
    {
        if (!_isApplicationQuitting)
        {
            Instantiate(_prefabDestroyParticleSystem, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().Play();
            _audioDeath.transform.parent = null;
            _audioDeath.Play();

            int aliveVirus = GameObject.FindObjectsOfType<VirusController>().Length;

            if (aliveVirus == 0)
            {
                GlitchController.Instance.StopGlitch();
            }
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