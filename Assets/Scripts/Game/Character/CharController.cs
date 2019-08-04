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
    #region Classes & Struct
    [Serializable]
    class PlayerCollision
    {
        const string FORMAT = "^: {0}, v {1} \n {2} < > {3}";

        public bool left = false;
        public bool right = false;
        public bool up = false;
        public bool down = false;

        public PlayerCollision() : this(false, false, false, false)
        {
        }

        public PlayerCollision(bool left, bool right, bool up, bool down)
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
    #endregion

    #region Enum
    private enum SpecialState
    {
        None = 0,
        Sticked,
        Tackle
    }
    #endregion

    #region Fields
    #region static readonly
    public readonly static int MAX_JUMPS_COUNT = 2;
    public readonly static float TACKLE_DURATION = 1.1f / 2f;
    public readonly static float RAYCAST_DISTANCE = 0.1f;
    public readonly static float TACKLE_TRANSITION_TIME = 0.1f;
    #endregion

    #region serialized variables
    public CharID playerId;
    [Space]
    [SerializeField] private CharacterControllerData _data;
    [Header("Attacking")]
    [SerializeField] private GameObject _prefabProjectile;
    [SerializeField] private Vector3 _projectileOrigin;
    [Header("Collisions")]
    [SerializeField] private Collider2D _normalCollider;
    [SerializeField] private Collider2D _tackleCollider;
    [SerializeField, Tooltip("Collider between normal & tackle collider")] private Collider2D _transitionCollider;
    [Header("Rendering")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _crown;
    #endregion

    #region internals variables
    // state variables
    private CharControls _controls;
    private PlayerCollision _collision;
    private int _jumpsCount = 0;
    private float _horizontalVelocity = 0;
    private bool _isMVP;
    private bool _canThrowProjectile = true;
    [EditorBrowsable(EditorBrowsableState.Never)] private SpecialState _state = SpecialState.None;
    [EditorBrowsable(EditorBrowsableState.Never)] private int _directionX = 1;
    [EditorBrowsable(EditorBrowsableState.Never)] private Collider2D _currentCollider;

    // attack variables
    private List<Entity> _entitiesHit = new List<Entity>();

    // input
    private float _horizontalInput = 0;
    private bool _jumpPressed = false;
    private bool _tacklePressed = false;
    private bool _throwPressed = false;

    // caching variables
    private Rigidbody2D _rb;

    private int _layerMask;

    private readonly int _hashAttack = Animator.StringToHash("attack");
    private readonly int _hashJump = Animator.StringToHash("jump");
    private readonly int _hashTackle = Animator.StringToHash("tackle");
    private readonly int _hashWallSliding = Animator.StringToHash("wall_sliding");
    private readonly int _hashRunning = Animator.StringToHash("running");
    #endregion
    #endregion

    #region Properties
    #region Private
    private int DirectionX
    {
        get
        {
            return _directionX;
        }

        set
        {
            _directionX = Mathf.Clamp(value, -1, 1);

            switch (_directionX)
            {
                case -1:
                    _spriteRenderer.flipX = true;
                    break;

                case 1:
                    _spriteRenderer.flipX = false;
                    break;
            }

            // update collider direction
            float offsetX = Mathf.Abs(_tackleCollider.offset.x) * _directionX * -1;
            _tackleCollider.offset = new Vector2(offsetX, _tackleCollider.offset.y);

            offsetX = Mathf.Abs(_transitionCollider.offset.x) * _directionX * -1;
            _transitionCollider.offset = new Vector2(offsetX, _transitionCollider.offset.y);
        }
    }

    private SpecialState State
    {
        get
        {
            return _state;
        }

        set
        {
            if (_state == SpecialState.Tackle)
                return;

            if (value == SpecialState.Tackle)
            {
                CurrentCollider = _transitionCollider;

                this.ExecuteAfterTime(TACKLE_TRANSITION_TIME, () =>
                {
                    CurrentCollider = _tackleCollider;
                });

                this.ExecuteAfterTime(TACKLE_DURATION, () =>
                {
                    CurrentCollider = _normalCollider;

                    _state = SpecialState.None;
                    _entitiesHit.Clear();
                });
            }

            _state = value;
        }
    }

    private Collider2D CurrentCollider
    {
        set
        {
            _currentCollider = value;

            if (_currentCollider == _tackleCollider)
            {
                _tackleCollider.enabled = true;
                _normalCollider.enabled = false;
                _transitionCollider.enabled = false;
            }
            else if (_currentCollider == _normalCollider)
            {
                _normalCollider.enabled = true;
                _tackleCollider.enabled = false;
                _transitionCollider.enabled = false;
            }
            else if (_currentCollider == _transitionCollider)
            {
                _transitionCollider.enabled = true;
                _normalCollider.enabled = true;
                _tackleCollider.enabled = false;
            }
        }

        get
        {
            return _currentCollider;
        }
    }

    private bool CanThrowProjectile
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
    #endregion

    #region Public
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
    #endregion
    #endregion

    #region MonoBehaviour callbacks
    #region Initialization
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        CurrentCollider = _tackleCollider;

        _layerMask = ~LayerMask.GetMask("Entity", "Ignore Collision");

        AirConsole.instance.onMessage += HandleInput;
    }

    void Start()
    {
        _spriteRenderer.color = playerId.ToColor();
        _controls = playerId.ToControls();
        _collision = new PlayerCollision();
        _crown.enabled = false;
    }
    #endregion

    #region Tick
    void Update()
    {
        UpdateCollisions();

        ManageStick();
        ManageJumpCount();

        ProcessAttacksInputs();

#if UNITY_EDITOR
        HandleKeyboardInput();
#endif
    }

    void FixedUpdate()
    {
        UpdateCollisions();
        ProcessMovementInputs();
    }

    void LateUpdate()
    {
        // update Animator state
        _animator.SetBool(_hashWallSliding, (State == SpecialState.Sticked));
        _animator.SetBool(_hashRunning, (_horizontalInput != 0));
        _animator.SetBool(_hashJump, !_collision.down);
        _animator.SetBool(_hashTackle, (State == SpecialState.Tackle));
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
        if (State == SpecialState.Tackle)
        {
            Entity otherEntity = other.gameObject.GetComponent<Entity>();

            if (otherEntity != null)
            {
                bool isEntityHittedPreviously = (_entitiesHit.Find(x => x == otherEntity) != null);

                if (!isEntityHittedPreviously)
                {
                    otherEntity.GetDamage(_data.DamageTackle, GetComponent<Entity>());
                    _entitiesHit.Add(otherEntity);
                }
            }
        }
    }
    #endregion
    #endregion

    #region Tick Methods
    #region Update
    void UpdateCollisions()
    {
        float distY = CurrentCollider.bounds.extents.y + RAYCAST_DISTANCE;
        float distX = CurrentCollider.bounds.extents.x + RAYCAST_DISTANCE;

        Vector3 position = CurrentCollider.bounds.center;

        _collision.up = Physics2D.Raycast(position, Vector3.up, distY, _layerMask);
        _collision.down = Physics2D.Raycast(position, Vector3.down, distY, _layerMask);
        _collision.left = Physics2D.Raycast(position, Vector3.left, distX, _layerMask);
        _collision.right = Physics2D.Raycast(position, Vector3.right, distX, _layerMask);
    }

    void ManageJumpCount()
    {
        // reset jump count ?
        if (_collision.down || State == SpecialState.Sticked)
        {
            _jumpsCount = 0;
        }
    }

    void ManageStick()
    {
        if (!_collision.down && (_horizontalVelocity < 0 && _collision.left) || (_horizontalVelocity > 0 && _collision.right))
        {
            if (State != SpecialState.Tackle)
            {
                State = SpecialState.Sticked;
            }
        }
        else
        {
            if (State == SpecialState.Sticked)
            {
                State = SpecialState.None;
            }
        }
    }

    void ProcessAttacksInputs()
    {
        if (_throwPressed && _canThrowProjectile && State == SpecialState.None)
        {
            CanThrowProjectile = false;

            var projectile = Instantiate(_prefabProjectile, transform.position + _projectileOrigin, Quaternion.identity).GetComponent<Projectile>();

            projectile.damage = _data.DamageProjectile;
            projectile.Direction = Vector3.right * DirectionX;
            projectile.sender = GetComponent<Entity>();
        }

        if (_tacklePressed && State != SpecialState.Sticked)
        {
            State = SpecialState.Tackle;
        }
    }
    #endregion

    #region FixedUpdate
    void ProcessMovementInputs()
    {
        ProcessHorizontalInput();
        ProcessVerticalInput();
    }

    void ProcessHorizontalInput()
    {
        // set velocity
        if (State != SpecialState.Tackle)
        {
            _horizontalVelocity = _horizontalInput;
        }
        else if (_horizontalVelocity == 0)
        {
            _horizontalVelocity = DirectionX;
        }

        // ... added to velocity ...
        if (State != SpecialState.Sticked || (_horizontalVelocity < 0 && _collision.left == false) || (_horizontalVelocity > 0 && _collision.right == false))
        {
            _rb.velocity = new Vector2(_data.Speed * _horizontalVelocity, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }

        if (_horizontalInput != 0)
        {
            DirectionX = (int)_horizontalVelocity;
        }
    }

    void ProcessVerticalInput()
    {
        if (State == SpecialState.Sticked)
        {
            _rb.gravityScale = 0;
            _rb.velocity = new Vector2(_rb.velocity.x, _data.SlidingDownSpeed);
        }
        else
        {
            _rb.gravityScale = 1;

            if (_jumpPressed && State != SpecialState.Tackle && _jumpsCount < MAX_JUMPS_COUNT)
            {
                _jumpPressed = false;

                Jump();
            }
        }
    }

    void Jump()
    {
        _jumpsCount++;

        _rb.velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
        _rb.AddForce(Vector2.up * _data.JumpForce);
    }
    #endregion
    #endregion

    void HandleInput(int device_id, JToken data)
    {
        int playerNumber = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

        if (playerNumber == -1 || playerNumber != (int)playerId)
            return;

        if (data["horizontal"] != null)
        {
            _horizontalInput = (float)data["horizontal"];
        }

        if (data["bPressed"] != null)
        {
            _tacklePressed = (bool)data["bPressed"];
        }

        if (data["aPressed"] != null)
        {
            _jumpPressed = (bool)data["aPressed"];
        }

        if (data["xPressed"] != null)
        {
            _throwPressed = (bool)data["xPressed"];
        }
    }

#if UNITY_EDITOR
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(_controls.Right)) _horizontalInput = 1;
        if (Input.GetKeyUp(_controls.Right) && _horizontalInput != -1) _horizontalInput = 0;
        if (Input.GetKeyDown(_controls.Left)) _horizontalInput = -1;
        if (Input.GetKeyUp(_controls.Left) && _horizontalInput != 1) _horizontalInput = 0;

        if (Input.GetKeyDown(_controls.Jump)) _jumpPressed = true;
        if (Input.GetKeyUp(_controls.Jump)) _jumpPressed = false;

        if (Input.GetKeyDown(_controls.Throw)) _throwPressed = true;
        if (Input.GetKeyUp(_controls.Throw)) _throwPressed = false;

        if (Input.GetKeyDown(_controls.Tackle)) _tacklePressed = true;
        if (Input.GetKeyUp(_controls.Tackle)) _tacklePressed = false;
    }
#endif

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = playerId.ToColor();
        Gizmos.DrawSphere(transform.position + _projectileOrigin, 0.05f);
    }
}
