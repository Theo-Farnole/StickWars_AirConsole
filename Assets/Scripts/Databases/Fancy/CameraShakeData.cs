using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Camera Settings")]
public class CameraShakeData : ScriptableObject
{
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _magnitude = 0.3f;

    public float Duration { get => _duration; }
    public float Magnitude { get => _magnitude;  }
}
