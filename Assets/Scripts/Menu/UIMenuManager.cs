using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuManager : Singleton<UIMenuManager>
{
    #region Structs
    [System.Serializable]
    public class NoEnoughPlayers
    {
        public TextMeshProUGUI text;

        [NonSerialized] public Vector3 startingPosition;
        [NonSerialized] public Vector3 targetPosition;

        public GameObject gameObject { get => text.gameObject; }
        public Transform transform { get => text.transform; }

        public void Init(Vector3 positionDelta)
        {
            startingPosition = text.transform.position;
            targetPosition = text.transform.position + positionDelta;
        }
    }
    #endregion
    #region Fields
    [Header("Panel Lobby")]
    [SerializeField] private GameObject _panelLevelSelection;
    [Header("Panel Lobby > No enough players")]
    [SerializeField] private NoEnoughPlayers _noEnoughPlayers;
    [Space]
    [SerializeField, Range(0, 100), Tooltip("In percent of Screen height")] private float _deltaPosition = 5;
    [SerializeField] private float _fadeDuration = 1f;
    [Header("Panel Lobby > Players")]
    [SerializeField] private TextMeshProUGUI _textWaitingForPlayers;
    [SerializeField] private PlayerWrapper[] _playersWrappers = new PlayerWrapper[4];
    [Header("Panel Loading")]
    [SerializeField] private GameObject _panelLoading;
    [SerializeField] private TextMeshProUGUI _textGameInstruction;

    private Coroutine _coroutineNoEnoughPlayers;

    private Vector3 _startingPositionTextNoEnoughtPlayers;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _noEnoughPlayers.Init(Vector3.down * ((_deltaPosition / 100f) * Screen.height));
    }

    void Start()
    {
        _panelLevelSelection.SetActive(true);
        _panelLoading.SetActive(false);
        _noEnoughPlayers.gameObject.SetActive(false);

        if (!AirConsole.instance.IsAirConsoleUnityPluginReady())
        {
            // hide avatar until player join
            for (int i = 0; i < _playersWrappers.Length; i++)
            {
                _playersWrappers[i].transform.ActionForEachChildren((GameObject c) =>
                {
                    c.SetActive(false);
                });
            }
        }        
    }
    #endregion

    #region Panel Level Selection
    public void UpdatePlayersAvatar()
    {

        foreach(CharId charId in Enum.GetValues(typeof(CharId)))
        {
            int deviceId = CharIdAllocator.GetDeviceId(charId);
            bool isPlayerActive = false;
            isPlayerActive = CharIdAllocator.DoDeviceIdExist(deviceId);

            ActivePlayerAvatar(isPlayerActive, charId, deviceId);
        }

        // display text waiting or not
        bool shouldDisplayTextWaiting = (AirConsole.instance.GetControllerDeviceIds().Count == 0);
        _textWaitingForPlayers.gameObject.SetActive(shouldDisplayTextWaiting);
    }

    private void ActivePlayerAvatar(bool active, CharId charId, int deviceId)
    {
        int index = (int)charId;

        _playersWrappers[index].Outline.color = charId.GetUIColor();
        _playersWrappers[index].Name.text = AirConsole.instance.GetNickname(deviceId);
        ProfilePictureManager.Instance.SetProfilePicture(deviceId, _playersWrappers[index].Avatar);
        
        _playersWrappers[index].transform.ActionForEachChildren((GameObject child) =>
        {
            child.SetActive(active);
        });
    }

    public void DisplayNoEnoughPlayersText(bool active)
    {
        _noEnoughPlayers.gameObject.SetActive(true);

        // start Coroutine
        if (_coroutineNoEnoughPlayers != null) StopCoroutine(_coroutineNoEnoughPlayers);

        _coroutineNoEnoughPlayers = new Timer(this, _fadeDuration, (float f) =>
        {
            var color = _noEnoughPlayers.text.color;
            color.a = 1 - f;
            _noEnoughPlayers.text.color = color;

            _noEnoughPlayers.transform.position = Vector3.Lerp(_noEnoughPlayers.startingPosition, _noEnoughPlayers.targetPosition, f);
        }, () =>
        {
            _noEnoughPlayers.gameObject.SetActive(false);
            _noEnoughPlayers.transform.position = _noEnoughPlayers.startingPosition;
        }).Coroutine;

    }
    #endregion

    #region Panel Loading
    public void SetActivePanelLoading()
    {
        _textGameInstruction.text = _textGameInstruction.text.Replace("$value$", MenuManager.Instance.SelectedGamemodeDefaultValue.ToString());

        _panelLevelSelection.SetActive(false);
        _panelLoading.SetActive(true);
    }
    #endregion
    #endregion
}

