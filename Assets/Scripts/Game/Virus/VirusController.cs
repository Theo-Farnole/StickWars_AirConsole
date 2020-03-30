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
    private Dictionary<CharacterEntity, float> _hitCharacterEntityByTime = new Dictionary<CharacterEntity, float>();
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
    #region MonoBehaviour Callbacks
    #region Initialization
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
    #endregion

    #region Tick
    void Update()
    {
        _state?.Tick();
    }

    void FixedUpdate()
    {
        _state?.FixedTick();
    }
    #endregion

    #region Collision
    void OnTriggerStay2D(Collider2D collision) => OnCollision(collision);
    void OnTriggerEnter2D(Collider2D collision) => OnCollision(collision);
    #endregion

    #region Miscellaneous
    void OnDestroy()
    {
        // prevent Instantiating while we are quitting the game
        if (!_isApplicationQuitting)
        {
            Instantiate(_prefabDestroyParticleSystem, transform.position, Quaternion.identity).GetComponent<ParticleSystem>().Play();
            _audioDeath.transform.parent = null;
            _audioDeath.Play();
        }
    }

    void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }
    #endregion

    #region Debug
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
    #endregion

    #region On Collision manager
    void OnCollision(Collider2D collision)
    {
        var characterEntity = collision.GetComponent<CharacterEntity>();

        // prevent inflict dmaage to non character entity
        if (characterEntity == null)
            return;

        if (!_hitCharacterEntityByTime.ContainsKey(characterEntity))
            _hitCharacterEntityByTime.Add(characterEntity, 0);

        if (_hitCharacterEntityByTime[characterEntity] <= Time.time)
        {
            _hitCharacterEntityByTime[characterEntity] = Time.time + _data.DelayBetweenAttackInflict;

            characterEntity.GetDamage(_data.AttackDamage, _entity, AttackType.VirusHit);
            _audioDoDamage.Stop();
            _audioDoDamage.Play();
        }
    }
    #endregion
    #endregion
}