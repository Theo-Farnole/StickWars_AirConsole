using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Entity : MonoBehaviour
{
    #region Fields
    [Header("Entity Config")]
    [SerializeField] protected EntityData _entityData;

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
        _hp -= damage;

        if (_hp < 0)
        {
            _hp = 0;
        }

        if (!IsAlive())
        {
            Death();
        }
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
    }
}
