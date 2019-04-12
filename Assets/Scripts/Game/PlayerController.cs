using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    #region Fields
    [SerializeField] private PlayerID _playerId;
    [Space]
    [Header("Movements")]
    [SerializeField] private float _speed = 3;
    [SerializeField] private float _jumpForce = 500;
    [Header("Attack")]
    [SerializeField] private int _damageTackle = 3;
    [Header("Rendering")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    private bool _isGrounded = false;
    private bool _isTackling = false;

    private int _hashAttack, _hashJump, _hashRunning, _hashTackle, _hashWallSliding;
    #endregion

    #region MonoBehaviour callbacks
    void Start()
    {
        _spriteRenderer.color = _playerId.ToColor();

        _hashJump = Animator.StringToHash("jump");
        _hashRunning = Animator.StringToHash("running");
        _hashTackle = Animator.StringToHash("tackle");
        _hashWallSliding = Animator.StringToHash("wall_sliding");
    }

    void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");

        transform.position += horizontal * _speed * Vector3.right * Time.deltaTime;
        transform.localScale = new Vector2(Mathf.Sign(horizontal), 1); // to face the direction

        CheckIsGrounded();
        ManageInput();
    
        // update Animator
        _animator.SetBool(_hashRunning, (horizontal != 0));
        _animator.SetBool(_hashJump, !_isGrounded);
    }

    #region OnCollision callbacks
    void OnCollisionEnter2D(Collision2D hit)
    {
        Entity ent = hit.gameObject.GetComponent<Entity>();

        if (ent != null)
        {
            ent.GetDamage(_damageTackle);
        }
    }
    #endregion
    #endregion

    #region Inputs
    void ManageInput()
    {
        _isTackling = Input.GetKey(KeyCode.S); 
        _animator.SetBool(_hashTackle, _isTackling);
        

        if (Input.GetKeyDown(KeyCode.Z) && _isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        _isGrounded = false;

        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * _jumpForce);
    }
    #endregion

    void CheckIsGrounded()
    {
        var min = GetComponent<BoxCollider2D>().bounds.min;

        RaycastHit2D hit2D = Physics2D.Raycast(min, Vector2.down, 0.1f);
        _isGrounded = hit2D;

        Debug.DrawLine(min, min + Vector3.down * 0.1f, _playerId.ToColor());
    }
}
