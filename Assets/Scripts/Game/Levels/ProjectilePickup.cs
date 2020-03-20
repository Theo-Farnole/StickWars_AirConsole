using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void ProjectilePickupDelegate(ProjectilePickup projectilePickup);

public class ProjectilePickup : MonoBehaviour, IPooledObject
{
    #region Fields    
    public UnityEvent OnPickup;

    // cache variable
    private FancyObject _fancyObject;
    #endregion

    #region Properties
    public FancyObject FancyObject
    {
        get
        {
            if (_fancyObject == null)
                _fancyObject = GetComponent<FancyObject>();

            return _fancyObject;
        }
    }
    #endregion

    #region Methods
    void OnTriggerEnter2D(Collider2D collision)
    {
        var hitCharController = collision.GetComponent<CharController>();

        if (hitCharController)
        {
            hitCharController.FillCarriedProjectilesAmount();
            OnPickup?.Invoke();

            ObjectPooler.Instance.EnqueueGameObject("projectile_pickup", gameObject);
        }
    }

    void IPooledObject.OnObjectSpawn()
    {
        Debug.LogFormat("On object spawn position {0}", transform.position);
        FancyObject.ResetStartingPosition(transform.position);
    }
    #endregion
}
