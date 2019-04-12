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
    [SerializeField] private int _damageFistAttack = 3;
    [SerializeField] private Transform hitBoxFistAttack;
    [Header("Rendering")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    private bool isGrounded = false;

    private int _hashAttack, _hashJump, _hashRunning, _hashTackle, _hashWallSliding;
    #endregion

    #region MonoBehaviour callbacks
    void Start()
    {
        _spriteRenderer.color = _playerId.ToColor();

        _hashAttack = Animator.StringToHash("attack");
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

        if (horizontal == 0)
        {
            _animator.SetBool(_hashRunning, false);
        }
        else
        {
            _animator.SetBool(_hashRunning, true);
        }

        Debug.Log("horizontal = " + horizontal);

        ManageInput();
    }

    void LateUpdate()
    {

    }
    #region OnCollision callbacks
    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckIsGrounded();
    }

    void OnCollisionExit(Collision collision)
    {
        CheckIsGrounded();
    }
    #endregion
    #endregion

    #region Inputs
    void ManageInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Attack();
            _animator.SetTrigger(_hashAttack);
        }

        if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            Jump();
            _animator.SetTrigger(_hashJump);
        }
    }

    void Attack()
    {
        var hitBoxPointA = new Vector2(hitBoxFistAttack.position.x, hitBoxFistAttack.position.y) + new Vector2(hitBoxFistAttack.localScale.x, hitBoxFistAttack.localScale.y) * 0.5f;
        var hitBoxPointB = new Vector2(hitBoxFistAttack.position.x, hitBoxFistAttack.position.y) - new Vector2(hitBoxFistAttack.localScale.x, hitBoxFistAttack.localScale.y) * 0.5f;
        var hit = Physics2D.OverlapArea(hitBoxPointA, hitBoxPointB);

        if (hit != null && hit.gameObject.tag == "Enemy")
        {
            Debug.Log("Attaque " + hit.gameObject.name);
            hit.gameObject.GetComponent<Entity>().GetDamage(_damageFistAttack);
        }
    }

    void Jump()
    {
        isGrounded = false;

        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * _jumpForce);
    }
    #endregion

    void CheckIsGrounded()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position - transform.localScale / 2 + Vector3.down * 0.15f, Vector2.down, 0.1f);
        isGrounded = hit2D;

        Debug.Log("isGrounded -> " + isGrounded);
    }
}
