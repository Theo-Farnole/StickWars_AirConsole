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

    private PlayerControls _controls;

    private bool _isGrounded = false;
    private bool _isTackling = false;

    private int _hashAttack, _hashJump, _hashRunning, _hashTackle, _hashWallSliding;
    #endregion

    #region MonoBehaviour callbacks
    void Start()
    {
        _spriteRenderer.color = _playerId.ToColor();
        _controls = _playerId.ToControls();

        _hashJump = Animator.StringToHash("jump");
        _hashRunning = Animator.StringToHash("running");
        _hashTackle = Animator.StringToHash("tackle");
        _hashWallSliding = Animator.StringToHash("wall_sliding");
    }

    void Update()
    {
        int horizontal = 0;
        
        horizontal = Input.GetKey(_controls.Left) ?  horizontal-1 : horizontal;
        horizontal = Input.GetKey(_controls.Right) ? horizontal+1 : horizontal;

        transform.position += horizontal * _speed * Vector3.right * Time.deltaTime;
        transform.localScale = new Vector2(Mathf.Sign(horizontal), 1); // to face the direction

        CheckIsGrounded();
        ManageInput();

        // update Animator
        _animator.SetBool(_hashRunning, (horizontal != 0));
        _animator.SetBool(_hashJump, !_isGrounded);
        _animator.SetBool(_hashTackle, _isTackling);
    }

    #region OnCollision callbacks
    private void OnTriggerEnter2D(Collider2D hit)
    {
        Entity ent = hit.gameObject.GetComponent<Entity>();

        if (ent != null && _isTackling)
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
        Vector3 raycastPosition = new Vector3(GetComponent<BoxCollider2D>().bounds.center.x, GetComponent<BoxCollider2D>().bounds.min.y - 0.05f, 0f);

        RaycastHit2D hit2D = Physics2D.Raycast(raycastPosition, Vector2.down, 0.05f);
        _isGrounded = hit2D;

        Debug.DrawLine(raycastPosition, raycastPosition + Vector3.down * 0.05f, _playerId.ToColor());
    }
}
