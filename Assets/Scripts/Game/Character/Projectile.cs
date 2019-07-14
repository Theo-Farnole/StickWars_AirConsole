using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// Projectile look at direction
// Player can stick to projectile
    
[SelectionBase]
public class Projectile : MonoBehaviour
{
    #region Fields
    [SerializeField] private ProjectileData _data;

    [HideInInspector] public int damage = 10;
    [HideInInspector] public Entity sender;

    private Vector3 _direction = Vector3.right;
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
        }
    }
    #endregion

    void Update()
    {
        transform.position += _direction * _data.Speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.GetComponent<Entity>();

        if (entity != null && entity != sender)
        {
            entity.GetDamage(damage, sender);
            Destroy(gameObject);
        }
    }
}
