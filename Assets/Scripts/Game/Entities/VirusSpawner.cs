using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public delegate void VirusSpawnerDelegate(VirusSpawner virusController);

public class VirusSpawner : Entity
{
    #region Fields
    [Header("Main settings")]
    [SerializeField] private GameObject _prefabVirus;
    [SerializeField] private int _deathCountToReleaseVirus = 3;
    [Header("Interface")]
    [SerializeField] private GameObject _sliderCanvas;
    [SerializeField] private Slider[] _healthSliders = new Slider[3];    
    [Header("Feedbacks")]
    [SerializeField] private AudioSource _audioDeath;    
    [SerializeField] private float _glitchEffectDuration = 1f;    
    [Header("Debug")]
    [SerializeField] private bool _debugAttackEveryCharacter = false;
    [SerializeField] private bool _debugInstantKill = false;

    private bool _isApplicationQuitting = false;

    private int _currentDeathCount = 0;
    #endregion

    #region Methods
#if !UNITY_EDITOR
    protected override void Awake()
    {
        base.Awake();

        _debugAttackEveryCharacter = false;
        _debugInstantKill = false;
    }
#endif

    void Start()
    {
        for (int i = 0; i < _healthSliders.Length; i++)
        {
            _healthSliders[i].maxValue = MaxHp;
            _healthSliders[i].value = MaxHp;
        }

        SetCurrentHealthBar();
        _sliderCanvas.SetActive(false);
    }

    public override void GetDamage(int damage, Entity attacker)
    {
        base.GetDamage(damage, attacker);

        bool shouldHideCanvas = (_currentDeathCount == 0 && _hp == MaxHp);
        _sliderCanvas.SetActive(!shouldHideCanvas);
    }

    protected override void Death(Entity killer)
    {
        _currentDeathCount++;
        SetCurrentHealthBar();

        // VirusTriggerer has reach last position
        if (_currentDeathCount >= _deathCountToReleaseVirus || _debugInstantKill)
        {
            TriggerVirus(killer);

            CameraShake.Instance.Shake(0.3f, 0.15f);
            CameraEffectController.Instance.EnableGlitch(true);

            CameraEffectController.Instance.ExecuteAfterTime(_glitchEffectDuration, ()
                => CameraEffectController.Instance.EnableGlitch(false));
        }
        else
        {
            var newPosition = LevelDataLocator.GetLevelData().GetRandomVirusSpawnerPosition(transform.position);
            GetComponent<FancyObject>()?.ResetStartingPosition(newPosition);

            _hp = MaxHp;
            UpdateHealthSlider();

            CameraShake.Instance.Shake();
        }
    }

    void TriggerVirus(Entity killer)
    {
        // spawn a virus that target every player, except the killer of VirusTriggerer
        foreach (CharId item in Enum.GetValues(typeof(CharId)))
        {
            if (GameManager.Instance.Characters.ContainsKey(item) && GameManager.Instance.Characters[item] != null)
            {
                var charController = GameManager.Instance.Characters[item];

                if (charController.charId != killer.GetComponent<CharController>()?.charId || _debugAttackEveryCharacter)
                {
                    var virus = Instantiate(_prefabVirus, transform.position, Quaternion.identity);
                    virus.GetComponent<VirusController>().target = charController.transform;
                }
            }
        }

        Destroy(gameObject);
    }

    void SetCurrentHealthBar()
    {
        if (_currentDeathCount < _healthSliders.Length)
        {
            _healthSlider = _healthSliders[_currentDeathCount];
        }
        else
        {
            Debug.LogWarning("No Health Slider for " + _currentDeathCount + "th death of " + transform.name);
        }
    }

    void OnDestroy()
    {
        if (!_isApplicationQuitting)
        {
            _audioDeath.transform.parent = null;
            _audioDeath.Play();
        }
    }

    void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }
    #endregion
}

