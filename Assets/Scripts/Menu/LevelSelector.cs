using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : Singleton<LevelSelector>
{
    public readonly static float CHANGE_LEVEL_TIME_DELAY = 0.3f;

    #region Classes & Struct
    [System.Serializable]
    public struct LevelData
    {
        public string key;
        public RectTransform rectTransform;
    }
    #endregion

    #region Fields
    [Header("Level selection")]
    [SerializeField] private RectTransform _cursor;
    [SerializeField] private LevelData[] _levels;

    private int _selectedLevel = 0;
    private int _inputHorizontal = 0;

    private bool _canChangeLevel = true;
    #endregion

    #region Properties
    private bool CanChangeLevel
    {
        get
        {
            return _canChangeLevel;
        }

        set
        {
            if (value == true)
                return;

            _canChangeLevel = false;

            this.ExecuteAfterTime(CHANGE_LEVEL_TIME_DELAY, () =>
            {
                _canChangeLevel = true;
            });
        }
    }

    private int SelectedLevel
    {
        get
        {
            return _selectedLevel;
        }

        set
        {
            value = MyMath.InverseClamp(value, 0, _levels.Length - 1);
            _selectedLevel = value;
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void Update()
    {
        UpdateCursor();

        if (_inputHorizontal != 0 && _canChangeLevel)
        {
            CanChangeLevel = false;
            SelectedLevel += _inputHorizontal;

            UpdateCursor();
        }
    }
    #endregion

    #region AirConsole Callbacks
    void OnMessage(int deviceId, JToken data)
    {
        if (deviceId == AirConsole.instance.GetMasterControllerDeviceId())
        {
            if (data["horizontal"] != null)
            {
                _inputHorizontal = (int)data["horizontal"];
            }
        }
    }
    #endregion

    void UpdateCursor()
    {
        // update position
        _cursor.localPosition = _levels[_selectedLevel].rectTransform.localPosition;

        Debug.Log(_levels[_selectedLevel].rectTransform.localPosition);

        // update color
        if (_levels[_selectedLevel].key == "lock")
        {
            _cursor.GetComponent<Image>().color = Color.red;
        }
        else
        {
            _cursor.GetComponent<Image>().color = Color.white;
        }
    }

    public LevelData GetSelectedLevelData()
    {
        return _levels[_selectedLevel];
    }
    #endregion
}
