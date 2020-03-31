using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Level/Level Layout")]
public class LevelLayoutData : ScriptableObject
{
    [Header("MAIN SETTINGS")]
    [SerializeField] private bool _enableLevelLayout = true;
    [SerializeField] private float _sumRatioToLoadLayout = (3f / 8f);

    public bool EnableLevelLayout { get => _enableLevelLayout; }
    public float SumRatioToLoadLayout { get => _sumRatioToLoadLayout; }
}
