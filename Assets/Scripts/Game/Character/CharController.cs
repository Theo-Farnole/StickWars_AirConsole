using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using UnityEngine;

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

            offsetX = Mathf.Abs(_transitionCollider.offset.x) * (int)o* -1;
            _transitionCollider.offset = new Vector2(offsetX, _transitionCollider.offset.y);
        }
    }

    public class PlayerInputs
    {
        public int horizontalInput = 0;
        public bool jumpPressed = false;
        public bool tacklePressed = false;
        public bool throwPressed = false;

        public void Reset()
        {
            horizontalInput = 0;
            jumpPressed = false;
            tacklePressed = false;
            throwPressed = false;
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

    #region serialized variables
    public CharID charID;
    [Space]
    [SerializeField] private CharacterControllerData _data;
    [Header("Attacking")]
    [SerializeField] private GameObject _prefabProjectile;
    [SerializeField] private Vector3 _projectileOrigin;
    [Header("Collisions")]
    [SerializeField] private CharacterCollisions _collisions;
    [Header("Rendering")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _crown;
    #endregion

    #region internals variables
    private CharacterRaycast _raycast = new CharacterRaycast();
    private CharState _state;

    private CharControls _keyboardControls;
    private bool _isMVP = false;
    private bool _canThrowProjectile = true;
    private Orientation _orientationX = Orientation.Right;

    // attack variables
    private List<Entity> _entitiesHit = new List<Entity>();
    private PlayerInputs _inputs = new PlayerInputs();

    #region cache variables
    private Rigidbody2D _rigidbody;
    private CharAudio _charAudio;

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

    public CharState State
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
    #endregion

    #region MonoBehaviour callbacks
    #region Initialization
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _charAudio = GetComponent<CharAudio>();
        _layerMask = ~LayerMask.GetMask("Entity", "Ignore Collision", "Ignore Raycast");

        _collisions.SetCollider(CharacterCollisions.Collider.Normal);

        State = new CharStateNormal(this);
        AirConsole.instance.onMessage += HandleInput;
    }

    void Start()
    {
        _spriteRenderer.color = charID.ToColor();
        _keyboardControls = charID.ToControls();
        _crown.enabled = false;
    }
    #endregion

    #region Tick
    void Update()
    {
#if UNITY_EDITOR
        HandleKeyboardInput();
#endif

        UpdateCollisions();
        State?.Tick();
    }

    void FixedUpdate()
    {
        UpdateCollisions();
        State?.FixedTick();
    }

    void LateUpdate()
    {
        _animator.SetBool(_hashRunning, (_inputs.horizontalInput != 0));
        _animator.SetBool(_hashJump, !_raycast.down);

        if (_state != null)
        {
            _animator.SetBool(_hashWallSliding, _state is CharStateSticked);
            _animator.SetBool(_hashTackle, _state is CharStateTackle);
        }
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

                    _charAudio.PlaySound(CharAudio.Sound.HitTackle);
                }
            }
        }
    }
    #endregion
    #endregion

    #region Tick Methods
    void UpdateCollisions()
    {
        float distY = _collisions.CurrentCollider.bounds.extents.y + RAYCAST_DISTANCE;
        float distX = _collisions.CurrentCollider.bounds.extents.x + RAYCAST_DISTANCE;

        Vector3 position = _collisions.CurrentCollider.bounds.center;

        _raycast.up = Physics2D.Raycast(position, Vector3.up, distY, _layerMask);
        _raycast.down = Physics2D.Raycast(position, Vector3.down, distY, _layerMask);
        _raycast.left = Physics2D.Raycast(position, Vector3.left, distX, _layerMask);
        _raycast.right = Physics2D.Raycast(position, Vector3.right, distX, _layerMask);
    }
    #endregion

    public void ThrowProjectile()
    {
        CanThrowProjectile = false;

        var projectile = Instantiate(_prefabProjectile, transform.position + _projectileOrigin, Quaternion.identity).GetComponent<Projectile>();

        projectile.damage = _data.DamageProjectile;
        projectile.Direction = Vector3.right * (int)OrientationX;
        projectile.sender = GetComponent<Entity>();
    }

    public void Respawn()
    {
        StopAllCoroutines();

        _inputs.Reset();
        _entitiesHit.Clear();

        OrientationX = Orientation.Right;
        _collisions.SetCollider(CharacterCollisions.Collider.Normal);

        _canThrowProjectile = true;

        State = new CharStateNormal(this);
    }

    #region Inputs Management
    void HandleInput(int device_id, JToken data)
    {
        int playerNumber = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

        if (playerNumber == -1 || playerNumber != (int)charID)
            return;

        if (data["horizontal"] != null)
        {
            _inputs.horizontalInput = (int)data["horizontal"];
        }

        if (data["bPressed"] != null)
        {
            _inputs.tacklePressed = (bool)data["bPressed"];
        }

        if (data["aPressed"] != null)
        {
            _inputs.jumpPressed = (bool)data["aPressed"];
        }

        if (data["xPressed"] != null)
        {
            _inputs.throwPressed = (bool)data["xPressed"];
        }
    }

#if UNITY_EDITOR
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(_keyboardControls.Right)) _inputs.horizontalInput = 1;
        if (Input.GetKeyUp(_keyboardControls.Right) && _inputs.horizontalInput != -1) _inputs.horizontalInput = 0;
        if (Input.GetKeyDown(_keyboardControls.Left)) _inputs.horizontalInput = -1;
        if (Input.GetKeyUp(_keyboardControls.Left) && _inputs.horizontalInput != 1) _inputs.horizontalInput = 0;

        if (Input.GetKeyDown(_keyboardControls.Jump)) _inputs.jumpPressed = true;
        if (Input.GetKeyUp(_keyboardControls.Jump)) _inputs.jumpPressed = false;

        if (Input.GetKeyDown(_keyboardControls.Throw)) _inputs.throwPressed = true;
        if (Input.GetKeyUp(_keyboardControls.Throw)) _inputs.throwPressed = false;

        if (Input.GetKeyDown(_keyboardControls.Tackle)) _inputs.tacklePressed = true;
        if (Input.GetKeyUp(_keyboardControls.Tackle)) _inputs.tacklePressed = false;
    }
#endif
    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = charID.ToColor();
        Gizmos.DrawSphere(transform.position + _projectileOrigin, 0.05f);
    }
}
