﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class Entity : MonoBehaviour
{
    #region Fields
    [Header("Entity Config")]
    [SerializeField] protected EntityData _entityData;
    [SerializeField] protected Slider _healthSlider;

    private int _hp;
    private int _maxHp;
    #endregion

    #region Properties
    public int MaxHp { get => _maxHp; }
    public int Hp { get => _hp; }
    #endregion

    protected virtual void Start()
    {
        if (_entityData != null)
        {
            _hp = _entityData.Hp;
            _maxHp = _entityData.Hp;

            UpdateHealthSlider();
        }
        else
        {
            Debug.Log("Il manque une entity data pour " + transform.name);
        }
    }

    /**
     * Reduce entity's HP.
     */
    virtual public void GetDamage(int damage)
    {
        Debug.Log(transform.name + " a reçu " + damage);

        _hp -= damage;
        _hp = Mathf.Clamp(_hp, 0, _maxHp);

        UpdateHealthSlider();

        if (!IsAlive())
        {
            Death();
        }
    }

    private void UpdateHealthSlider()
    {
        if (_healthSlider == null)
            return;

        // active or not the slider
        bool isFullLife = (_hp == _maxHp);
        _healthSlider.gameObject.SetActive(!isFullLife);

        // update the value
        _healthSlider.maxValue = _maxHp;
        _healthSlider.value = _hp;
    }

    public bool IsAlive()
    {
        if (_hp <= 0)
            return false;

        else
            return true;
    }

    virtual protected void Death()
    {
        Destroy(gameObject);
    }
}
