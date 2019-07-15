using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


// TODO
// Create variable for player orientation (instead of using sprite orientation)
// Add cadence in projectile throw
// Create enum for SpecialState { Sticked, Tackling, None }

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

    #region Fields
    #region static readonly
    public readonly static int MAX_JUMPS_COUNT = 2;
    public readonly static float TACKLE_DURATION = 1.1f / 2f;
    public readonly static float RAYCAST_DISTANCE = 0.1f;
    #endregion

    #region serialized variables
    public CharID playerId;
    [Space]
    [SerializeField] private PlayerControllerData _data;
    [Header("Attacking")]
    [SerializeField] private GameObject _prefabProjectile;
    [SerializeField] private Vector3 _projectileOrigin;
    [Header("Rendering")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _crown;
    #endregion

    #region internals variables
    // state variables
    private CharControls _controls;
    private PlayerCollision _collision;
    private bool _isStick = false;
    private int _jumpsCount = 0;
    private float _horizontalVelocity = 0;
    private bool _isMVP;
    private bool _canThrow = true;

    // attack variables
    private List<Entity> _entitiesHit = new List<Entity>();

    // input
    private float _horizontalInput = 0;
    private bool _jumpPressed = false;
    private bool _tacklePressed = false;
    private bool _throwPressed = false;

    // caching variables
    private Rigidbody2D _rb;
    private Collider2D _collider;

    private int _layerMask;

    private readonly int _hashAttack = Animator.StringToHash("attack");
    private readonly int _hashJump = Animator.StringToHash("jump");
    private readonly int _hashTackle = Animator.StringToHash("tackle");
    private readonly int _hashWallSliding = Animator.StringToHash("wall_sliding");
    private readonly int _hashRunning = Animator.StringToHash("running");
    #endregion
    #endregion

    #region Properties
    private bool TacklePressed
    {
        get
        {
            return _tacklePressed;
        }

        set
        {
            // can't externally set _tacklePressed to false
            if (value == false)
                return;

            _horizontalVelocity = _spriteRenderer.flipX ? -1 : 1;
            _tacklePressed = value;

            this.ExecuteAfterTime(TACKLE_DURATION, () =>
            {
                _tacklePressed = false;
                _entitiesHit.Clear();
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

    public bool CanThrow
    {
        get
        {
            return _canThrow;
        }

        set
        {
            if (value == true)
                return;

            _canThrow = false;

            this.ExecuteAfterTime(_data.CadenceProjectile, () =>
            {
                _canThrow = true;
            });
        }
    }
    #endregion

    #region MonoBehaviour callbacks
    #region Initialization
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

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

        ProcessThrowInput();

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
        _animator.SetBool(_hashWallSliding, _isStick);
        _animator.SetBool(_hashRunning, (_horizontalInput != 0));
        _animator.SetBool(_hashJump, !_collision.down);
        _animator.SetBool(_hashTackle, _tacklePressed);
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
    void OnCollisionEnter2D(Collision2D collision)
    {
        // sticking system
        bool isStickOld = _isStick;

        // if we just get sticked, reset velocity
        if (_isStick && !isStickOld)
        {
            _rb.velocity = _isStick ? Vector2.zero : _rb.velocity;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!_tacklePressed)
            return;

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

    private void OnTriggerExit2D(Collider2D other)
    {
        Entity otherEntity = other.gameObject.GetComponent<Entity>();

        if (otherEntity != null)
        {
            _entitiesHit.Remove(otherEntity);
        }
    }
    #endregion
    #endregion

    #region Tick Methods
    #region Update
    void UpdateCollisions()
    {
        float distY = _collider.bounds.extents.y + RAYCAST_DISTANCE;
        float distX = _collider.bounds.extents.x + RAYCAST_DISTANCE;

        Vector3 position = _collider.bounds.center;

        _collision.up = Physics2D.Raycast(position, Vector3.up, distY, _layerMask);
        _collision.down = Physics2D.Raycast(position, Vector3.down, distY, _layerMask);
        _collision.left = Physics2D.Raycast(position, Vector3.left, distX, _layerMask);
        _collision.right = Physics2D.Raycast(position, Vector3.right, distX, _layerMask);
    }

    void ManageJumpCount()
    {
        // reset jump count ?
        if (_collision.down || _isStick)
        {
            _jumpsCount = 0;
        }
    }

    void ManageStick()
    {
        // sticking system
        if (!_collision.down && (_horizontalVelocity < 0 && _collision.left) || (_horizontalVelocity > 0 && _collision.right))
        {
            _isStick = true;
        }
        else
        {
            _isStick = false;
        }
    }

    void ProcessThrowInput()
    {
        if (_throwPressed && _canThrow)
        {
            _throwPressed = false;
            CanThrow = false;

            var projectile = Instantiate(_prefabProjectile, transform.position + _projectileOrigin, Quaternion.identity).GetComponent<Projectile>();

            projectile.damage = _data.DamageProjectile;
            projectile.Direction = Vector3.right * (_spriteRenderer.flipX ? -1 : 1);
            projectile.sender = GetComponent<Entity>();
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
        if (!_tacklePressed)
        {
            _horizontalVelocity = _horizontalInput;
        }

        // ... added to velocity ...
        if (!_isStick || (_horizontalVelocity < 0 && _collision.left == false) || (_horizontalVelocity > 0 && _collision.right == false))
        {
            _rb.velocity = new Vector2(_data.Speed * _horizontalVelocity, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }

        // ... modify face of the sprite
        if (_horizontalVelocity != 0)
        {
            _spriteRenderer.flipX = (_horizontalVelocity < 0) ? true : false;
        }
    }

    void ProcessVerticalInput()
    {
        if (_isStick)
        {
            _rb.gravityScale = 0;
            _rb.velocity = new Vector2(_rb.velocity.x, _data.SlidingDownSpeed);
        }
        else
        {
            _rb.gravityScale = 1;

            if (_jumpPressed && !_tacklePressed && _jumpsCount < MAX_JUMPS_COUNT)
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
            TacklePressed = (bool)data["bPressed"];
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

        if (Input.GetKeyDown(_controls.Tackle)) TacklePressed = true;
    }
#endif

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = playerId.ToColor();
        Gizmos.DrawSphere(transform.position + _projectileOrigin, 0.05f);
    }
}
