using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VirusSpawner : Entity
{
    #region Fields
    [Header("Virus Config")]
    [SerializeField] private GameObject _prefabVirus;
    [SerializeField] private GameObject _sliderCanvas;
    [SerializeField] private Slider[] _healthSliders = new Slider[3];
    [Space]
    [SerializeField] private AudioSource _audioDeath;
    [Space]
    [SerializeField] private bool _debugAttackEveryCharacter = false;
    [SerializeField] private bool _debugInstantKill = false;

    private bool _isApplicationQuitting = false;

    private Transform[] _positions;
    private int _deathCount = 0;
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
        Transform[] currentPositionArray = new Transform[] { transform };
        _positions = currentPositionArray.Union(LevelData.Instance.VirusSpawnerPosition).ToArray();

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

        bool shouldHideCanvas = (_deathCount == 0 && _hp == MaxHp);
        _sliderCanvas.SetActive(!shouldHideCanvas);
    }

    protected override void Death(Entity killer)
    {
        _deathCount++;
        SetCurrentHealthBar();

        // VirusTriggerer has reach last position
        if (_deathCount >= _positions.Length || _debugInstantKill)
        {
            TriggerVirus(killer);

            CameraShake.Instance.Shake(0.3f, 0.15f);
            CameraEffectController.Instance.EnableGlitch(true);
        }
        else
        {
            GetComponent<FancyObject>()?.ResetStartingPosition(_positions[_deathCount].position);

            _hp = MaxHp;
            UpdateHealthSlider();

            CameraShake.Instance.Shake();
        }
    }

    void TriggerVirus(Entity killer)
    {
        if (killer.GetComponent<CharController>() == null)
        {
            Debug.LogWarning(transform.name + " has been killed by a non CharController named " + killer.name);
            return;
        }

        // spawn a virus that target every player, except the killer of VirusTriggerer
        for (int i = 0; i < GameManager.Instance.Characters.Length; i++)
        {
            var charController = GameManager.Instance.Characters[i];

            if (charController != null)
            {
                if (charController.charID != killer.GetComponent<CharController>()?.charID || _debugAttackEveryCharacter)
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
        if (_deathCount < _healthSliders.Length)
        {
            _healthSlider = _healthSliders[_deathCount];
        }
        else
        {
            Debug.LogWarning("No Health Slider for " + _deathCount + "th death of " + transform.name);
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

