using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TO Databases/Projectile")]
public class ProjectileData : MonoBehaviour
{
    [SerializeField] private int _speed = 3;

    public int Speed { get => _speed; }
}
