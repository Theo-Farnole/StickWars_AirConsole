using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirusTriggerer : Entity
{
    #region Fields
    [Header("Virus Config")]
    [SerializeField] private GameObject _prefabVirus;
    [Space]
    [SerializeField] private bool _debugAttackEveryCharacter = false;
    [SerializeField] private bool _debugInstantKill = false;

    private Transform[] _positions;
    private int _deathCount = 0;
    #endregion

    #region Methods
    void Awake()
    {
        Transform[] currentPositionArray = new Transform[] { transform };
        _positions = currentPositionArray.Union(LevelData.Instance.VirusTriggererPosition).ToArray();

#if UNITY_EDITOR
#else
        _debugAttackEveryCharacter = false;
        _debugInstantKill = false;
#endif
    }

    protected override void Death(Entity killer)
    {
        _deathCount++;

        // VirusTriggerer has reach last position
        if (_deathCount >= _positions.Length || _debugInstantKill)
        {
            TriggerVirus(killer);

            CameraShake.Instance.Shake(0.3f, 0.15f);
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
    #endregion
}

