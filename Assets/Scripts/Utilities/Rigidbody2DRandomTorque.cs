using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Rigidbody2DRandomTorque : MonoBehaviour
{
    [SerializeField] private float _torqueForce;

    void Start()
    {
        ApplyTorque();
    }

    public void ApplyTorque()
    {
        float torque = Random.Range(-_torqueForce, _torqueForce);

        GetComponent<Rigidbody2D>().AddTorque(torque, ForceMode2D.Impulse);

        Debug.LogFormat("Applied torque is {0}", torque);
    }
}
