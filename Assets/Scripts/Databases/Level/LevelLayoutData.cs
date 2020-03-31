using System.Collections;
using System.Collections.Generic;
using TF.Utilities.RemoteConfig;
using UnityEngine;

[CreateAssetMenu(menuName = "StickWars/Level/Level Layout")]
public class LevelLayoutData : RemoteConfigScriptableObject
{
    [Header("MAIN SETTINGS")]
    [SerializeField] private bool _enableLevelLayout = true;
    [Tooltip("0.5f means that the MVP is a the half of the kill goal.")]
    [SerializeField, Range(0, 1)] private float _mvpKillProgressToLoadLayout = 0.5f;

    public bool EnableLevelLayout { get => _enableLevelLayout; }
    public float MvpKillProgressToLoadLayout { get => _mvpKillProgressToLoadLayout; }
}
