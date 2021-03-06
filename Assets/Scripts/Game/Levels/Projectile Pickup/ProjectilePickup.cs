﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

public delegate void ProjectilePickupDelegate(ProjectilePickup projectilePickup);

public class ProjectilePickup : MonoBehaviour, IPooledObject
{
    #region Fields    
    public UnityEvent OnSpawn;
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
            TakePickup(hitCharController);
        }
    }    

    void Start()
    {
        ExtendedAnalytics.SendEvent("Projectile Pickup Spawn");
    }

    void OnDestroy()
    {
        ExtendedAnalytics.SendEvent("Projectile Pickup Destroy");
    }

    void TakePickup(CharController takerCharController)
    {
        int ammoBeforeTaking = takerCharController.CurrentAmountProjectilesCarried;
        ExtendedAnalytics.SendEvent("Projectile Pickup Taken", new Dictionary<string, object>()
        {
            { "Taker Char ID", takerCharController.charId },
            { "Ammo Before Taking", ammoBeforeTaking }
        });

        takerCharController.FillCarriedProjectilesAmount();
        OnPickup?.Invoke();

        ObjectPooler.Instance.EnqueueGameObject("projectile_pickup", gameObject);
    }

    void IPooledObject.OnObjectSpawn()
    {
        FancyObject.ResetStartingPosition(transform.position);

        ExtendedAnalytics.SendEvent("Projectile Pickup Spawn");

        OnSpawn?.Invoke();
    }
    #endregion
}
