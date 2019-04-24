using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    #region Fields
    #region serialized variables
    [SerializeField] private PlayerID _playerId;
    [Space]
    [SerializeField] private PlayerControllerData _data;
    [Header("Rendering")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;
    #endregion

    #region internals variables
    const int MAX_JUMPS_COUNT = 2;
    const float TACKLE_DURATION = 1.1f / 2f;
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

    int _horizontal = 0;

    private PlayerControls _controls;
    private PlayerCollision _collision;
    private bool _isGrounded = false;
    private bool _isTackling = false;
    private bool _isStick = false;
    private int _jumpsCount = 0;

    // caching variables
    private int _hashAttack, _hashJump, _hashRunning, _hashTackle, _hashWallSliding;
    private Rigidbody2D _rb;
    private Vector3 _velocity = Vector3.zero;
    #endregion
    #endregion

    #region MonoBehaviour callbacks
    void Start()
    {
        _spriteRenderer.color = _playerId.ToColor();
        _controls = _playerId.ToControls();
        _collision = new PlayerCollision();

        // caching variables
        _rb = GetComponent<Rigidbody2D>();

        _hashJump = Animator.StringToHash("jump");
        _hashRunning = Animator.StringToHash("running");
        _hashTackle = Animator.StringToHash("tackle");
        _hashWallSliding = Animator.StringToHash("wall_sliding");
    }

    void Update()
    {
        // inputs
        ManageInput();

        // update isGrounded var
        _isGrounded = _collision.down;

        // reset jump count ?
        if (_isGrounded)
        {
            _jumpsCount = 0;
        }
    }

    // update Animator state
    void LateUpdate()
    {
        _animator.SetBool(_hashWallSliding, _isStick);
        _animator.SetBool(_hashRunning, (_horizontal != 0));
        _animator.SetBool(_hashJump, !_isGrounded);
        _animator.SetBool(_hashTackle, _isTackling);
    }

    // update Rigidbody2D force
    void FixedUpdate()
    {
        // movement
        HorizontalMove();
        VerticalMove();
    }
    #endregion

    #region OnCollision callbacks
    void OnCollisionEnter2D(Collision2D collision)
    {
        // ===
        // sticking system
        bool isStickOld = _isStick;

        UpdateCollisions();    

        // if we just get sticked ...
        if (_isStick && !isStickOld)
        {
            // ... reset velocity
            _rb.velocity = _isStick ? Vector2.zero : _rb.velocity;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        UpdateCollisions();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.name == "player blue")
        {
            Debug.Log("OnTriggerEnter2D with " + other.transform.name);
        }

        Entity ent = other.gameObject.GetComponent<Entity>();

        if (ent != null && _isTackling)
        {
            Debug.Log("<color=green>" + transform.name + "</color> hit with <color=red>" + other.transform.name + "</color> + other.collider.IsTrigger? " + other.isTrigger);
            ent.GetDamage(_data.DamageTackle);
        }
    }
    #endregion

    #region Update Methods
    void ManageInput()
    {
        // tackle system
        //_isTackling = Input.GetKey(_controls.Tackle);

        if (Input.GetKeyDown(_controls.Tackle))
        {
            _isTackling = true;

            StartCoroutine(ExecuteAfterTime(TACKLE_DURATION, () =>
            {
                _isTackling = false;
            }));
        }

        // jump
        if (Input.GetKeyDown(_controls.Jump) && !_isTackling && _jumpsCount < MAX_JUMPS_COUNT)
        {
            Jump();
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

        // sticking system
        if (!_isGrounded && (_collision.left  && _horizontal < 0) ||
                            (_collision.right && _horizontal > 0))
        {
            _isStick = true;
            _jumpsCount = 0;
        }
        else
        {
            _isStick = false;
        }
    }

    void HorizontalMove()
    {
        // ... override horizontal if he's talcking ...
        if (_isTackling)
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

    void VerticalMove()
    {
        if (_isStick)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _data.SlidingDownSpeed);
            _rb.gravityScale = 0;
        }
        else
        {
            _rb.gravityScale = 1;
        }
    }

    void Jump()
    {
        _jumpsCount++;
        _isGrounded = false;

        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * _data.JumpForce);
    }
    #endregion

    void UpdateCollisions()
    {
        Vector3 raycastPosition;
        RaycastHit2D hit2D;
        int layerMask = ~LayerMask.GetMask("Entity", "Ignore Collision");

        #region UP
        raycastPosition = new Vector3(GetComponent<BoxCollider2D>().bounds.center.x, GetComponent<BoxCollider2D>().bounds.max.y, 0f);
        hit2D = Physics2D.Raycast(raycastPosition, Vector2.up, 0.05f, layerMask);

        _collision.up = hit2D;
        Debug.DrawLine(raycastPosition, raycastPosition + Vector3.up * 0.05f, _playerId.ToColor());
        #endregion

        #region LEFT
        raycastPosition = new Vector3(GetComponent<BoxCollider2D>().bounds.min.x, GetComponent<BoxCollider2D>().bounds.center.y, 0f);
        hit2D = Physics2D.Raycast(raycastPosition, Vector2.left, 0.05f, layerMask);
        _collision.left = hit2D;

        Debug.DrawLine(raycastPosition, raycastPosition + Vector3.left * 0.05f, _playerId.ToColor());
        #endregion

        #region RIGHT
        raycastPosition = new Vector3(GetComponent<BoxCollider2D>().bounds.max.x, GetComponent<BoxCollider2D>().bounds.center.y, 0f);
        hit2D = Physics2D.Raycast(raycastPosition, Vector2.right, 0.05f, layerMask);
        _collision.right = hit2D;

        Debug.DrawLine(raycastPosition, raycastPosition + Vector3.right * 0.05f, _playerId.ToColor());
        #endregion

        #region DOWN
        raycastPosition = new Vector3(GetComponent<BoxCollider2D>().bounds.center.x, GetComponent<BoxCollider2D>().bounds.min.y, 0f);
        hit2D = Physics2D.Raycast(raycastPosition, Vector2.down, 0.05f, layerMask);
        _collision.down = hit2D;

        Debug.DrawLine(raycastPosition, raycastPosition + Vector3.down * 0.05f, _playerId.ToColor());
        #endregion

        //Debug.Log( _collision.ToString());
    }

    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);

        task();
    }
}
