using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    public readonly static float CHANGE_LEVEL_TIME_DELAY = 0.3f;

    #region Enum
    public enum State
    {
        PlayerPanel,
        GamemodePanel,
        Loading
    }
    #endregion

    #region Fields
    [EnumNamedArray(typeof(GamemodeType))]
    [SerializeField] private GamemodeData[] _gamemodeData = new GamemodeData[Enum.GetValues(typeof(GamemodeType)).Length];

    private bool _canChangeLevel = true;
    private GamemodeType _selectedGamemode = GamemodeType.DeathMatch;

    private State _currentState = State.PlayerPanel;
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

    public State CurrentState { get => _currentState; }
    public GamemodeData[] GamemodeData { get => _gamemodeData; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;

        if (AirConsole.instance.IsAirConsoleUnityPluginReady())
        {
            OnReady(string.Empty);
        }
        else
        {
            AirConsole.instance.onReady += OnReady;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            NextPressed();
        }

        HorizontalPressed(Input.GetAxis("Horizontal"));
    }
#endif 

    void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onConnect -= OnConnect;
            AirConsole.instance.onMessage -= OnMessage;
            AirConsole.instance.onDisconnect -= OnDisconnect;
            AirConsole.instance.onReady -= OnReady;
        }
    }
    #endregion

    #region AirConsole Callbacks
    void OnMessage(int device_id, JToken data)
    {
        if (device_id == AirConsole.instance.GetMasterControllerDeviceId())
        {
            if (data["horizontal"] != null)
            {
                HorizontalPressed((float)data["horizontal"]);
            }

            if (data["aPressed"] != null && (bool)data["aPressed"])
            {
                NextPressed();
            }
        }
    }

    void OnConnect(int device_id)
    {
        int activePlayers = AirConsole.instance.GetActivePlayerDeviceIds.Count;
        if (activePlayers < GameManager.MAX_PLAYERS)
        {
            AirConsole.instance.SetActivePlayers(activePlayers + 1);
        }

        UIMenuManager.Instance.UpdatePlayersAvatar();
        UpdateControllerView();
    }

    void OnReady(string str)
    {
        UIMenuManager.Instance.UpdatePlayersAvatar();
        UpdateControllerView();
    }

    void OnDisconnect(int device_id)
    {
        // is part of active players ?
        if (AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id) != -1)
        {
            int activePlayers = AirConsole.instance.GetActivePlayerDeviceIds.Count;

            if (AirConsole.instance.GetControllerDeviceIds().Count >= GameManager.MAX_PLAYERS)
            {
                AirConsole.instance.SetActivePlayers(activePlayers);
            }
            else
            {
                AirConsole.instance.SetActivePlayers(activePlayers - 1);
            }
        }

        UIMenuManager.Instance.UpdatePlayersAvatar();
        UpdateControllerView();
    }
    #endregion

    void UpdateControllerView()
    {
        var playersNumber = AirConsole.instance.GetControllerDeviceIds().Count;

        for (int i = 0; i < playersNumber; i++)
        {
            var device_id = AirConsole.instance.GetControllerDeviceIds()[i];

            string bgColor = ((CharID)AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id)).ToString();
            string view = string.Empty;

            if (device_id == AirConsole.instance.GetMasterControllerDeviceId())
            {
                view = ControllerView.Play.ToString();
            }
            else if (AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id) == -1)
            {
                view = ControllerView.NoPlace.ToString();
            }
            else
            {
                view = ControllerView.Wait.ToString();
            }

            var token = new
            {
                view,
                bgColor
            };

            AirConsole.instance.Message(device_id, token);
        }
    }

    #region Handle Input
    void NextPressed()
    {
        _currentState++;

        UIMenuManager.Instance.UpdatePanel();

        // load scene
        if (_currentState == State.Loading)
        {
            // pass the gamemode settings 
            var gamemodeData = _gamemodeData[(int)_selectedGamemode];
            int sliderIndex = (int)UIMenuManager.Instance.SliderGamemodeSettings.value;
            AbstractGamemode.valueForVictory = gamemodeData.ValuesSettings[sliderIndex];

            // display play view on controllers
            var token = new
            {
                view = ControllerView.Play.ToString(),
            };

            AirConsole.instance.Broadcast(token);

            // then, finally load the scene
            SceneManager.LoadScene("SC_Windows");
        }
    }

    void HorizontalPressed(float value)
    {
        if (value == 0)
            return;

        switch (_currentState)
        {
            case State.PlayerPanel:
                if (_canChangeLevel)
                {
                    LevelSelector.Instance.SelectedLevel += (int)value;
                    CanChangeLevel = false;
                }
                break;

            case State.GamemodePanel:
                UIMenuManager.Instance.SliderGamemodeSettings.value += (int)value;
                break;
        }
    }
    #endregion
    #endregion
}
