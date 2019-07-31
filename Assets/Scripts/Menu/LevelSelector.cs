using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : Singleton<LevelSelector>
{
    #region Classes & Struct
    [System.Serializable]
    public struct LevelData
    {
        public string key;
        public Sprite sprite;
    }
    #endregion

    #region Fields
    [Header("Level selection")]
    [SerializeField] private LevelData[] _levels;
    [Space]
    [SerializeField] private Image _leftImageLevel;
    [SerializeField] private Image _selectedImageLevel;
    [SerializeField] private Image _rightImageLevel;

    private int _selectedLevel = 0;
    #endregion

    #region Properties
    public int SelectedLevel
    {
        get
        {
            return _selectedLevel;
        }

        set
        {
            //_selectedLevel = MyMath.InverseClamp(value, 0, _levels.Length - 1);
            UpdateUI();
        }
    }
    #endregion

    void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        Debug.LogWarning("Temporary broken function");
        //int leftIndex = MyMath.InverseClamp(_selectedLevel - 1, 0, _levels.Length - 1);
        //int rightIndex = MyMath.InverseClamp(_selectedLevel + 1, 0, _levels.Length - 1);

        //_selectedImageLevel.sprite = _levels[_selectedLevel].sprite;
        //_leftImageLevel.sprite = _levels[leftIndex].sprite;
        //_rightImageLevel.sprite = _levels[rightIndex].sprite;
    }

    public LevelData GetSelectedLevelData()
    {
        return _levels[_selectedLevel];
    }
}
