using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Projectile : MonoBehaviour
{
    #region Fields
    public static readonly float LIFETIME = 10f;
    public static readonly float STICKED_LIFETIME = 1f;

    [SerializeField] private ProjectileData _data;

    [HideInInspector] public int damage = 10;
    [HideInInspector] public Entity sender;

    private Vector3 _direction = Vector3.right;
    private bool _stickedInWall = false;

    private Rigidbody2D _rb;
    #endregion

    #region Properties
    public Vector3 Direction
    {
        get
        {
            return _direction;
        }

        set
        {
            _direction = value.normalized;
            GetComponentInChildren<SpriteRenderer>().flipX = _direction.x > 0 ? false : true;
        }
    }
    #endregion


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, LIFETIME);
    }

    void Update()
    {
        if (_stickedInWall)
            return;

        transform.position += _direction * _data.Speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Collision w/ " + collider.name);

        var entity = collider.GetComponent<Entity>();

        if (entity != null && entity != sender)
        {
            entity.GetDamage(damage, sender);
        }

        // destroy projectile on first collision, if touched sender
        if ((entity == null && collider.gameObject.layer != LayerMask.NameToLayer("Ignore Collision")) ||
            (entity != null && entity != sender))
        {
            StopProjectile();
        }
    }

    void StopProjectile()
    {
        _stickedInWall = true;
        Destroy(gameObject, STICKED_LIFETIME);

        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;
    }
}
