using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Virus : Entity
{
    #region Fields
    [SerializeField] private Transform[] _positions;

    private int _deathCount = 0;
    #endregion

    #region Methods
    void Awake()
    {
        Transform[] currentPositionArray = new Transform[] { transform };
        _positions = currentPositionArray.Union(_positions).ToArray();
    }

    protected override void Death(Entity killer)
    {
        _deathCount++;

        if (_deathCount < _positions.Length)
        {
            GetComponent<FancyObject>()?.ResetStartingPosition(_positions[_deathCount].position);

            _hp = MaxHp;
            UpdateHealthSlider();

            CameraShake.Instance.Shake();
        }
        else
        {
            TriggerVirus();

            CameraShake.Instance.Shake(0.3f, 0.15f);
        }
    }

    void TriggerVirus()
    {
        Debug.Log("VIRUS TRIGGERED");
        Destroy(gameObject);
    }
    #endregion
}
