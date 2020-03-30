using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public delegate void EntityDamage(Entity victim, int damageAmount);
public delegate void EntityDelegate(Entity entity);
public delegate void EntityEntityDelegate(Entity killer, Entity victim);
[System.Serializable] public class UnityEventEntityDamage : UnityEvent<Entity, int> { }

[SelectionBase]
public class Entity : MonoBehaviour
{
    private const Ease HEALTHBAR_ANIMATION_EASE = Ease.Linear;
    private const float HEALTHBAR_ANIMATION_DURATION = 0.1f;

    #region Fields
    public EntityDelegate OnHealthPointsChanged;
    public EntityEntityDelegate OnKillMade;
    public UnityEventEntityDamage OnDamage;    

    [Header("Entity Config")]
    [SerializeField] protected EntityData _entityData;
    public bool isInvincible = false;
    [Space]
    [SerializeField] private bool _hideHealthSliderIfFull = true;
    [SerializeField] protected Slider _healthSlider;

    protected List<AttackType> _attacksHistory = new List<AttackType>();

    protected int _hp;
    private int _maxHp;
    #endregion

    #region Properties
    public int MaxHp { get => _maxHp; }
    public int Hp { get => _hp; }
    public bool IsAlive { get => _hp > 0 ? true : false; }
    public bool IsFullLife { get => _hp == MaxHp; }
    public Slider HealthSlider { get => _healthSlider; }    
    public List<AttackType> AttacksHistory { get => _attacksHistory; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
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
    #endregion

    #region Damage Methods
    public virtual void GetDamage(int damage, Entity attacker, AttackType attackType)
    {
        if (isInvincible)
            return;
        AddToHistory(attackType);

        // remove HP
        _hp -= damage;
        _hp = Mathf.Clamp(_hp, 0, _maxHp);

        // damage feedbacks
        UpdateHealthSlider();
        PopFloatingText(damage, attacker);

        // check if alive
        if (!IsAlive)
        {
            Death(attacker);
        }

        // events
        OnDamage?.Invoke(this, damage);
        OnHealthPointsChanged?.Invoke(this);
    }

    public void Kill(Entity attacker, AttackType attackType)
    {
        GetDamage(_hp, attacker, attackType);
    }

    protected virtual void Death(Entity killer)
    {
        killer.OnKillMade?.Invoke(killer, this);
        Destroy(gameObject);
    }
    #endregion

    #region History methods
    protected void AddToHistory(AttackType attackType)
    {
        _attacksHistory.Add(attackType);
    }
    #endregion

    #region Damage Feedbacks
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
        _healthSlider.DOValue(_hp, HEALTHBAR_ANIMATION_DURATION).SetEase(HEALTHBAR_ANIMATION_EASE);
    }

    private void PopFloatingText(int damage, Entity attacker)
    {
        if (attacker == null)
            return;

        float direction = transform.position.x - attacker.transform.position.x;

        var floatingText = ObjectPooler.Instance.SpawnFromPool("floating_text", transform.position + Vector3.up * 1, Quaternion.identity).GetComponent<FloatingText>();
        floatingText.direction = (FloatingText.Direction)Mathf.Sign(direction);
        floatingText.Text.text = damage.ToString();

        if (attacker.GetComponent<CharController>())
        {
            floatingText.Text.color = attacker.GetComponent<CharController>().charId.GetSpriteColor();
        }
    }
    #endregion
    #endregion
}
