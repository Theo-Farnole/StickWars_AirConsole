using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public delegate void EntityDamage(Entity victim, int damageAmount);

[SelectionBase]
public class Entity : MonoBehaviour
{
    #region Fields
    public EntityDamage OnDamage;

    [Header("Entity Config")]
    [SerializeField] protected EntityData _entityData;
    public bool isInvincible = false;
    [Space]
    [SerializeField] private bool _hideHealthSliderIfFull = true;
    [SerializeField] protected Slider _healthSlider;


    protected int _hp;
    private int _maxHp;
    #endregion

    #region Properties
    public int MaxHp { get => _maxHp; }
    public int Hp { get => _hp; }
    public bool IsAlive { get => _hp > 0 ? true : false; }
    #endregion

    protected virtual void Awake()
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
    virtual public void GetDamage(int damage, Entity attacker)
    {
        if (isInvincible)
            return;

        _hp -= damage;
        _hp = Mathf.Clamp(_hp, 0, _maxHp);

        OnDamage?.Invoke(this, damage);

        UpdateHealthSlider();
        PopFloatingText(damage, attacker);

        if (!IsAlive)
        {
            Death(attacker);
        }
    }

    protected void UpdateHealthSlider()
    {
        if (_healthSlider == null)
        {
            Debug.LogWarning(transform.name + " doesn't have a health slider!");
            return;
        }

        // active or not the slider
        if (_hideHealthSliderIfFull)
        {
            bool isFullLife = (_hp == _maxHp);
            _healthSlider.gameObject.SetActive(!isFullLife);
        }

        // update the value
        _healthSlider.maxValue = _maxHp;
        _healthSlider.value = _hp;
    }

    protected virtual void Death(Entity killer)
    {
        Destroy(gameObject);
    }

    private void PopFloatingText(int damage, Entity attacker)
    {
        float direction = transform.position.x - attacker.transform.position.x;

        var floatingText = ObjectPooler.Instance.SpawnFromPool("floating_text", transform.position + Vector3.up * 1, Quaternion.identity).GetComponent<FloatingText>();        
        floatingText.direction = (FloatingText.Direction)Mathf.Sign(direction);
        floatingText.Text.text = damage.ToString();

        if (attacker.GetComponent<CharController>())
        {
            floatingText.Text.color = attacker.GetComponent<CharController>().charId.GetUIColor();
        }
    }
}
