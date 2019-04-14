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
    class PlayerCollision
    {
        private const string FORMAT = "^: {0}, v {1} \n {2} < > {3}";

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

    private PlayerControls _controls;
    private PlayerCollision _collision;
    private bool _isGrounded = false;
    private bool _isTackling = false;
    private bool _isStick = false;

    // caching variables
    private int _hashAttack, _hashJump, _hashRunning, _hashTackle, _hashWallSliding;
    private Rigidbody2D _rb;
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
        // horizontal input
        int horizontal = HorizontalMove();
        float vertical = VerticalMove();

        // inputs
        ManageInput();

        // update gravity scale
        _rb.gravityScale = _isStick ? 0f : 1f;
        _rb.velocity = _isStick ? Vector2.zero : _rb.velocity;

        // update Animator
        _animator.SetBool(_hashWallSliding, _isStick);
        _animator.SetBool(_hashRunning, (horizontal != 0));
        _animator.SetBool(_hashJump, !_isGrounded);
        _animator.SetBool(_hashTackle, _isTackling);
    }
    #endregion

    #region OnCollision callbacks
    void OnCollisionEnter2D(Collision2D collision)
    {
        bool isStickOld = _isStick;

        UpdateCollisions();    

        // if we just get sticked ...
        if (_isStick && !isStickOld)
        {
            // ... reset velocity
            _rb.velocity = _isStick ? Vector2.zero : _rb.velocity;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        UpdateCollisions();
    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        Entity ent = hit.gameObject.GetComponent<Entity>();

        if (ent != null && _isTackling)
        {
            ent.GetDamage(_data.DamageTackle);
        }
    }
    #endregion

    #region Update Functions

    /// <summary>
    /// Move the player by using it Rigidbody2D's velocity.
    /// </summary>
    /// <returns>horizontal axis</returns>
    int HorizontalMove()
    {
        // horizontal input...
        int horizontal = 0;

        horizontal = Input.GetKey(_controls.Left) ? horizontal - 1 : horizontal;
        horizontal = Input.GetKey(_controls.Right) ? horizontal + 1 : horizontal;

        // override horizontal if he's talcking
        if (_isTackling)
        {
            // horizontal in function of flipX
            horizontal = (_spriteRenderer.flipX ? -1 : 1); 
        }

        // ... added to velocity
        if (!_isStick || (horizontal < 0 && _collision.left == false) || (horizontal > 0 && _collision.right == false))
        {
            float delta = horizontal * _data.Speed * Time.deltaTime;

            transform.position += Vector3.right * delta;
        }

        // ... modify face of the sprite
        if (horizontal != 0)
        {
            _spriteRenderer.flipX = (horizontal < 0) ? true : false;
        }

        return horizontal;
    }

    float VerticalMove()
    {
        float vertical = 0f;

        if (_isStick)
        {
            vertical = _data.SlidingDownSpeed;
        }

        transform.position += Vector3.down * vertical * Time.deltaTime;

        return vertical;
    }

    void ManageInput()
    {
        _isTackling = Input.GetKey(_controls.Tackle);

        if (Input.GetKeyDown(_controls.Jump) && (_isGrounded || _isStick))
        {
            Jump();
        }
    }

    void Jump()
    {
        _isGrounded = false;

        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * _data.JumpForce);
    }

    void UpdateCollisions()
    {
        Vector3 raycastPosition;
        RaycastHit2D hit2D;
        int layerMask = ~LayerMask.GetMask("Entity");

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

        Debug.Log( _collision.ToString());

        _isGrounded = _collision.down;
        _isStick = !_collision.down && (_collision.left || _collision.right);

        if (_isStick)
        {
            Debug.Log("Sticked!");
        }
    }
    #endregion
}
