using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public delegate void CharControllerDelegate(CharController charController);
public delegate void CharControllerStateDelegate(CharController charController, AbstractCharState state);
public delegate void CharControllerIntDelegate(CharController charController, int integer);

public class CharController : MonoBehaviour
{
    #region Classes
    [Serializable]
    public class CharacterRaycast
    {
        const string FORMAT = "^: {0}, v {1} \n {2} < > {3}";

        public bool left = false;
        public bool right = false;
        public bool up = false;
        public bool down = false;

        public CharacterRaycast() : this(false, false, false, false)
        {
        }

        public CharacterRaycast(bool left, bool right, bool up, bool down)
        {
            this.left = left;
            this.right = right;
            this.up = up;
            this.down = down;
        }

        public override string ToString()
        {
            return string.Format(FORMAT, up, down, left, right);
        }
    }

    [Serializable]
    public class CharacterCollisions
    {
        public enum Collider
        {
            Normal,
            Transition,
            Tackle
        }

        private Collider2D _currentCollider;

        [SerializeField] private Collider2D _normalCollider;
        [SerializeField] private Collider2D _tackleCollider;
        [SerializeField, Tooltip("Collider between normal & tackle collider")] public Collider2D _transitionCollider;

        public Collider2D CurrentCollider { get => _currentCollider; }

        public void SetCollider(Collider c)
        {
            switch (c)
            {
                case Collider.Normal:
                    _currentCollider = _normalCollider;

                    _normalCollider.enabled = true;
                    _tackleCollider.enabled = false;
                    _transitionCollider.enabled = false;
                    break;
                case Collider.Transition:
                    _currentCollider = _transitionCollider;

                    _transitionCollider.enabled = true;
                    _normalCollider.enabled = false;
                    _tackleCollider.enabled = false;
                    break;
                case Collider.Tackle:
                    _currentCollider = _tackleCollider;

                    _tackleCollider.enabled = true;
                    _transitionCollider.enabled = false;
                    _normalCollider.enabled = false;
                    break;
            }
        }

        public void UpdateCollisionOffset(Orientation o)
        {
            float offsetX = Mathf.Abs(_tackleCollider.offset.x) * (int)o * -1;
            _tackleCollider.offset = new Vector2(offsetX, _tackleCollider.offset.y);

            offsetX = Mathf.Abs(_transitionCollider.offset.x) * (int)o * -1;
            _transitionCollider.offset = new Vector2(offsetX, _transitionCollider.offset.y);
        }
    }

    public class PlayerInputs
    {
        private int _horizontalInput = 0;
        private bool _jumpPressed = false;
        private bool _tacklePressed = false;
        private bool _throwPressed = false;

        private bool _throwDown = false;

        public int HorizontalInput { get => _horizontalInput; set => _horizontalInput = value; }
        public bool JumpPressed { get => _jumpPressed; set => _jumpPressed = value; }
        public bool TacklePressed { get => _tacklePressed; set => _tacklePressed = value; }
        public bool ThrowPressed
        {
            get => _throwPressed;

            set
            {
                _throwPressed = value;

                if (_throwPressed == true)
                {
                    _throwDown = true;
                }
            }
        }

        public bool ThrowDown { get => _throwDown; }

        /// <summary>
        /// Set "downs" variable as true
        /// </summary>
        public void Tick()
        {
            _throwDown = false;
        }

        public void Reset()
        {
            HorizontalInput = 0;
            JumpPressed = false;
            TacklePressed = false;
            ThrowPressed = false;
        }
    }
    #endregion

    #region Enum
    public enum Orientation
    {
        Left = -1,
        Right = 1
    }

    public enum SpecialState
    {
        None,
        Sticked,
        Tackle
    }
    #endregion

    #region Fields
    #region static readonly
    public readonly static int MAX_JUMPS_COUNT = 2;
    public readonly static float RAYCAST_DISTANCE = 0.1f;
    #endregion

    #region events
    public CharControllerDelegate OnDoubleJump;
    public CharControllerStateDelegate OnStateChanged;
    public CharControllerIntDelegate OnProjectileAmountUpdated;
    #endregion

    #region serialized variables
    public CharId charId;
    [Space]
    [SerializeField] private CharacterControllerData _data;
    [Header("Attacking")]
    [SerializeField] private GameObject _prefabProjectile;
    [Tooltip("When no projectile carried, we instanciate a emit a feedback.")]
    [SerializeField] private GameObject _prefabWhenNoProjectileCarried;
    [SerializeField] private Vector3 _projectileOrigin;
    [Header("Collisions")]
    [SerializeField] private CharacterCollisions _collisions;
    [Space]
    [SerializeField] private Collider2D _normalCollider;
    [SerializeField] private Collider2D _jumpingCollider;
    [Header("Rendering")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _crown;
    [Header("Events")]
    public UnityEvent NoCarriedProjectileOnThrow; // a bit too long, I apologize
    #endregion

    #region internals variables
    [HideInInspector] public int ownerDeviceId = -1;
    private bool _freeze;
    private CharacterRaycast _raycast = new CharacterRaycast();
    private AbstractCharState _state;

    private CharControls _keyboardControls;
    private bool _isMVP = false;
    private bool _canThrowProjectile = true;
    private Orientation _orientationX = Orientation.Left;

    private bool _badProjectileAlreadyInstanciatedThisFrame = false;

    // attack variables
    private int _currentAmountProjectilesCarried = 0;
    private List<Entity> _entitiesHit = new List<Entity>();
    private PlayerInputs _inputs = new PlayerInputs();

    #region cache variables
    private Rigidbody2D _rigidbody;
    private CharAudio _charAudio;
    private CharFeedback _charFeedback;

    private int _layerMask;

    private readonly int _hashAttack = Animator.StringToHash("attack");
    private readonly int _hashJump = Animator.StringToHash("jump");
    private readonly int _hashTackle = Animator.StringToHash("tackle");
    private readonly int _hashWallSliding = Animator.StringToHash("wall_sliding");
    private readonly int _hashRunning = Animator.StringToHash("running");
    #endregion
    #endregion
    #endregion

    #region Properties
    public Rigidbody2D Rigidbody { get => _rigidbody; }
    public CharacterControllerData Data { get => _data; }
    public CharacterRaycast Raycast { get => _raycast; }
    public CharacterCollisions Collisions { get => _collisions; }
    public PlayerInputs Inputs { get => _inputs; }
    public List<Entity> EntitiesHit { get => _entitiesHit; }
    public CharAudio CharAudio { get => _charAudio; }
    public CharFeedback CharFeedback { get => _charFeedback; }

    public bool Freeze
    {
        get
        {
            return _freeze;
        }

        set
        {
            _freeze = value;

            if (_freeze == true)
            {
                _inputs.Reset();
            }
        }
    }
    public Orientation OrientationX
    {
        get
        {
            return _orientationX;
        }

        set
        {
            _orientationX = value;

            switch (_orientationX)
            {
                case Orientation.Left:
                    _spriteRenderer.flipX = true;
                    break;

                case Orientation.Right:
                    _spriteRenderer.flipX = false;
                    break;
            }

            _collisions.UpdateCollisionOffset(_orientationX);
        }
    }

    public bool CanThrowProjectile
    {
        get
        {
            return _canThrowProjectile;
        }

        set
        {
            if (value == true)
                return;

            _canThrowProjectile = false;

            this.ExecuteAfterTime(_data.CadenceProjectile, () =>
            {
                _canThrowProjectile = true;
            });
        }
    }

    public bool IsMVP
    {
        get
        {
            return _isMVP;
        }

        set
        {
            _isMVP = value;
            _crown.enabled = _isMVP;
        }
    }

    public AbstractCharState State
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

            OnStateChanged?.Invoke(this, _state);
        }
    }

    public bool HasEnoughtCarriedProjectileToThrow { get => CurrentAmountProjectilesCarried > 0; }

    public SpriteRenderer SpriteRenderer { get => _spriteRenderer; set => _spriteRenderer = value; }
    public int CurrentAmountProjectilesCarried
    {
        get => _currentAmountProjectilesCarried;

        private set
        {
            _currentAmountProjectilesCarried = value;
            OnProjectileAmountUpdated?.Invoke(this, _currentAmountProjectilesCarried);
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour callbacks
    #region Initialization
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _charAudio = GetComponent<CharAudio>();
        _charFeedback = GetComponent<CharFeedback>();
        _layerMask = ~LayerMask.GetMask("Entity", "Ignore Collision", "Ignore Raycast");

        _collisions.SetCollider(CharacterCollisions.Collider.Normal);

        State = new CharStateNormal(this);
        AirConsole.instance.onMessage += HandleInput;
    }

    void Start()
    {
        _spriteRenderer.color = charId.GetSpriteColor();
        _keyboardControls = charId.ToControls();
        _crown.enabled = false;

        gameObject.name = gameObject.name.Replace("(Clone)", " " + charId.ToString());

        FillCarriedProjectilesAmount();

        NoCarriedProjectileOnThrow?.AddListener(ThrowBadProjectile);
    }
    #endregion

    #region Tick
    void Update()
    {
        _badProjectileAlreadyInstanciatedThisFrame = false;

#if UNITY_EDITOR
        HandleKeyboardInput();
#endif

        UpdateTriggerCollisions();
        UpdateHardCollisions();

        State?.Tick();
    }

    private void UpdateHardCollisions()
    {
        _normalCollider.enabled = _raycast.down;
        _jumpingCollider.enabled = !_raycast.down;
    }

    void FixedUpdate()
    {
        UpdateTriggerCollisions();
        State?.FixedTick();
    }

    void LateUpdate()
    {
        _animator.SetBool(_hashRunning, (_inputs.HorizontalInput != 0));
        _animator.SetBool(_hashJump, !_raycast.down);

        if (_state != null)
        {
            _animator.SetBool(_hashWallSliding, _state is CharStateSticked);
            _animator.SetBool(_hashTackle, _state is CharStateTackle);
        }

        _inputs.Tick();
    }
    #endregion

    #region Destroy
    void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= HandleInput;
        }
    }
    #endregion

    #region OnCollision callbacks
    void OnTriggerStay2D(Collider2D other)
    {
        if (_state is CharStateTackle)
        {
            Entity otherEntity = other.gameObject.GetComponent<Entity>();

            if (otherEntity != null)
            {
                bool isEntityHittedPreviously = (_entitiesHit.Find(x => x == otherEntity) != null);

                if (!isEntityHittedPreviously)
                {
                    otherEntity.GetDamage(_data.DamageTackle, GetComponent<Entity>());
                    _entitiesHit.Add(otherEntity);

                    _charAudio.PlayHitTackle();
                }
            }
        }
    }
    #endregion

    #region Miscellaneous
#if UNITY_EDITOR
    void OnValidate()
    {
        _spriteRenderer.color = charId.GetSpriteColor();
    }
#endif

    void OnDrawGizmosSelected()
    {
        Gizmos.color = charId.GetSpriteColor();
        Gizmos.DrawSphere(transform.position + _projectileOrigin, 0.05f);
    }
    #endregion
    #endregion

    #region Collisions
    void UpdateTriggerCollisions()
    {
        bool wasGrounded = _raycast.down;

        float distY = _collisions.CurrentCollider.bounds.extents.y + RAYCAST_DISTANCE;
        float distX = _collisions.CurrentCollider.bounds.extents.x + RAYCAST_DISTANCE;

        Vector3 position = _collisions.CurrentCollider.bounds.center;

        _raycast.up = Physics2D.Raycast(position, Vector3.up, distY, _layerMask);
        _raycast.down = Physics2D.Raycast(position, Vector3.down, distY, _layerMask);
        _raycast.left = Physics2D.Raycast(position, Vector3.left, distX, _layerMask);
        _raycast.right = Physics2D.Raycast(position, Vector3.right, distX, _layerMask);

        if (wasGrounded == false && _raycast.down)
        {
            HitGround();
        }
    }

    private void HitGround()
    {
        _charFeedback.PlayNonOrientedParticle(true, CharFeedback.Particle.HitGround);
        _charAudio.PlaySound(CharAudio.Sound.HitGround);
    }
    #endregion

    #region Attack methods
    public void ThrowProjectile()
    {
        if (_canThrowProjectile == false)
            return;

        CanThrowProjectile = false;

        // prevent throw projectile if not amount
        if (!HasEnoughtCarriedProjectileToThrow)
        {
            NoCarriedProjectileOnThrow?.Invoke();
            return;
        }

        CurrentAmountProjectilesCarried--;

        var gameObjectProjectile = ObjectPooler.Instance.SpawnFromPool("projectile", transform.position + _projectileOrigin, Quaternion.identity);
        var projectile = gameObjectProjectile.GetComponent<Projectile>();

        projectile.damage = _data.DamageProjectile;
        projectile.Direction = Vector3.right * (int)OrientationX;
        projectile.sender = GetComponent<Entity>();

        OnProjectileAmountUpdated?.Invoke(this, CurrentAmountProjectilesCarried);
    }

    public void ThrowBadProjectile()
    {
        if (_badProjectileAlreadyInstanciatedThisFrame)
            return;

        _badProjectileAlreadyInstanciatedThisFrame = true;

        var gameObject = GameObject.Instantiate(_prefabWhenNoProjectileCarried, transform.position + _projectileOrigin - Vector3.up * 0.3f, Quaternion.identity);

        gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.right * (int)OrientationX * 180);

        const float lifetime = 2.3f;
        Destroy(gameObject, lifetime);
    }

    public void FillCarriedProjectilesAmount()
    {
        CurrentAmountProjectilesCarried = _data.MaxProjectilesCarried;
    }
    #endregion

    #region Respawn methods
    public void Respawn()
    {
        // reset value
        StopAllCoroutines();

        _inputs.Reset();
        _entitiesHit.Clear();

        OrientationX = Orientation.Left;
        _collisions.SetCollider(CharacterCollisions.Collider.Normal);

        _canThrowProjectile = true;

        State = new CharStateNormal(this);
        GetComponent<CharacterEntity>().ResetHP();

        // feedback
        var deathPS = _charFeedback.GetNonOrientedParticle(CharFeedback.Particle.Death);
        Instantiate(deathPS, transform.position, Quaternion.identity).Play();

        _charAudio.PlaySound(CharAudio.Sound.Death);

        // set new position
        transform.position = LevelData.Instance.GetRandomSpawnPoint();
        _charFeedback.PlayRespawnParticle();

        _freeze = true;
        GetComponent<CharacterEntity>().isInvincible = true;

        this.ExecuteAfterTime(_data.RespawnDuration, () =>
        {
            _freeze = false;
            GetComponent<CharacterEntity>().isInvincible = false;
        });
    }
    #endregion

    #region Inputs Management
    void HandleInput(int deviceId, JToken data)
    {
        if (_freeze || deviceId != ownerDeviceId)
            return;

        if (data["horizontal"] != null)
        {
            _inputs.HorizontalInput = (int)data["horizontal"];
        }

        if (data["bPressed"] != null)
        {
            _inputs.TacklePressed = (bool)data["bPressed"];
        }

        if (data["aPressed"] != null)
        {
            _inputs.JumpPressed = (bool)data["aPressed"];
        }

        if (data["xPressed"] != null)
        {
            _inputs.ThrowPressed = (bool)data["xPressed"];
        }
    }

#if UNITY_EDITOR
    void HandleKeyboardInput()
    {
        if (_freeze)
            return;

        if (Input.GetKeyDown(_keyboardControls.Right)) _inputs.HorizontalInput = 1;
        if (Input.GetKeyUp(_keyboardControls.Right) && _inputs.HorizontalInput != -1) _inputs.HorizontalInput = 0;
        if (Input.GetKeyDown(_keyboardControls.Left)) _inputs.HorizontalInput = -1;
        if (Input.GetKeyUp(_keyboardControls.Left) && _inputs.HorizontalInput != 1) _inputs.HorizontalInput = 0;

        if (Input.GetKeyDown(_keyboardControls.Jump)) _inputs.JumpPressed = true;
        if (Input.GetKeyUp(_keyboardControls.Jump)) _inputs.JumpPressed = false;

        if (Input.GetKeyDown(_keyboardControls.Throw)) _inputs.ThrowPressed = true;
        if (Input.GetKeyUp(_keyboardControls.Throw)) _inputs.ThrowPressed = false;

        if (Input.GetKeyDown(_keyboardControls.Tackle)) _inputs.TacklePressed = true;
        if (Input.GetKeyUp(_keyboardControls.Tackle)) _inputs.TacklePressed = false;
    }
#endif
    #endregion
    #endregion
}
