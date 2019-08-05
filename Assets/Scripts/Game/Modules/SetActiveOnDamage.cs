using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnDamage : Entity
{
    [SerializeField] private GameObject[] _activeOnDamage = new GameObject[] { };

    void Awake()
    {
        for (int i = 0; i < _activeOnDamage.Length; i++)
        {
            _activeOnDamage[i].SetActive(false);
        }
    }

    public override void GetDamage(int damage, Entity attacker)
    {
        for (int i = 0; i < _activeOnDamage.Length; i++)
        {
            _activeOnDamage[i].SetActive(true);
        }
    }

    protected override void Death(Entity killer) { }
}
