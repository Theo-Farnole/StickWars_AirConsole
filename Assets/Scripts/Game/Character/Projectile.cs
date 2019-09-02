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

    private Rigidbody2D _rb;
    private bool _isFreeze = false;
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
            GetComponentInChildren<SpriteRenderer>().flipX = _direction.x < 0 ? false : true;
        }
    }
    #endregion

    #region Methods
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
        if (_isFreeze)
            return;

        transform.position += _direction * _data.Speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_isFreeze)
            return;

        var entity = other.GetComponent<Entity>();

        if (entity != null && entity != sender)
        {
            entity.GetDamage(damage, sender);
            entity.GetComponent<CharAudio>()?.PlaySound(CharAudio.Sound.HitProjectile);

            if (entity is CharacterEntity)
            {
                Destroy(gameObject);
            }
            else
            {
                _isFreeze = true;
                Destroy(gameObject, _data.LifetimeOnCollision);
            }
        }
        else
        {
            bool a = other.transform.CompareTag("Player");
            bool b = (other.transform.parent != null && other.transform.parent.CompareTag("Player"));

            if (!(a || b))
            {
                _isFreeze = true;
                Destroy(gameObject, _data.LifetimeOnCollision);
            }
        }
    }
    #endregion
}
