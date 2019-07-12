using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



public class CharController : MonoBehaviour
{
    #region Classes
    [System.Serializable]
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
    [SerializeField] private CharID _playerId;
    [Space]
    [SerializeField] private PlayerControllerData _data;
    [Header("Rendering")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    #endregion

    #region internals variables
    // state variables
    private CharControls _controls;
    private PlayerCollision _collision;
    private bool _isStick = false;
    private int _jumpsCount = 0;
    private Vector3 _velocity = Vector3.zero;

    // input
    private bool _tacklePressed = false;
    private int _horizontal = 0;
    private bool _jumpPressed = false;

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

    #region MonoBehaviour callbacks
    #region Initialization
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        _layerMask = ~LayerMask.GetMask("Entity", "Ignore Collision");
    }

    void Start()
    {
        _spriteRenderer.color = _playerId.ToColor();
        _controls = _playerId.ToControls();
        _collision = new PlayerCollision();
    }
    #endregion

    #region Tick
    void Update()
    {
        UpdateCollisions();

        ManageInput();
        ManageStick();
        ManageJump();
    }

    void FixedUpdate()
    {
        UpdateCollisions();

        ProcessInputs();
    }

    // update Animator state
    void LateUpdate()
    {
        _animator.SetBool(_hashWallSliding, _isStick);
        _animator.SetBool(_hashRunning, (_horizontal != 0));
        _animator.SetBool(_hashJump, !_collision.down);
        _animator.SetBool(_hashTackle, _tacklePressed);
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

    void OnTriggerEnter2D(Collider2D other)
    {
        Entity ent = other.gameObject.GetComponent<Entity>();

        if (ent != null && _tacklePressed)
        {
            Debug.Log("<color=green>" + transform.name + "</color> hit with <color=red>" + other.transform.name + "</color> + other.collider.IsTrigger? " + other.isTrigger);
            ent.GetDamage(_data.DamageTackle);
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

    void ManageInput()
    {
        if (Input.GetKeyDown(_controls.Tackle))
        {
            _tacklePressed = true;

            this.ExecuteAfterTime(TACKLE_DURATION, () =>
            {
                _tacklePressed = false;
            });
        }

        if (Input.GetKeyDown(_controls.Jump))
        {
            _jumpPressed = true;
        }

        if (Input.GetKeyUp(_controls.Jump))
        {
            _jumpPressed = false;
        }

        // horizontal input
        _horizontal = 0;

        if (Input.GetKey(_controls.Left))
        {
            _horizontal--;
        }

        if (Input.GetKey(_controls.Right))
        {
            _horizontal++;
        }
    }

    void ManageJump()
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
        if (!_collision.down && (_horizontal < 0 && _collision.left) || (_horizontal > 0 && _collision.right))
        {
            _isStick = true;
        }
        else
        {
            _isStick = false;
        }
    }
    #endregion

    #region FixedUpdate
    void ProcessInputs()
    {
        ProcessHorizontalInput();
        ProcessVerticalInput();
    }

    void ProcessHorizontalInput()
    {
        // ... override horizontal if he's talcking ...
        if (_tacklePressed)
        {
            // horizontal in function of flipX
            _horizontal = (_spriteRenderer.flipX ? -1 : 1);
        }

        // ... added to velocity ...
        if (!_isStick || (_horizontal < 0 && _collision.left == false) || (_horizontal > 0 && _collision.right == false))
        {
            _rb.velocity = new Vector2(_data.Speed * _horizontal, _rb.velocity.y);
        }
        else
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }

        // ... modify face of the sprite
        if (_horizontal != 0)
        {
            _spriteRenderer.flipX = (_horizontal < 0) ? true : false;
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
}
