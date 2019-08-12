using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class MenuManager : Singleton<MenuManager>
{
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

    private GamemodeType _selectedGamemode = GamemodeType.DeathMatch;
    private State _currentState = State.PlayerPanel;
    #endregion

    #region Properties
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
            APressed();
        }

        HorizontalPressed((int)Input.GetAxis("Horizontal"));
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
    void OnMessage(int deviceId, JToken data)
    {
        if (deviceId == AirConsole.instance.GetMasterControllerDeviceId())
        {
            if (data["horizontal"] != null)
            {
                HorizontalPressed((float)data["horizontal"]);
            }

            if (data["aPressed"] != null && (bool)data["aPressed"])
            {
                APressed();
            }
        }
    }

    void OnConnect(int deviceId)
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
    void APressed()
    {
        _currentState++;

        switch (_currentState)
        {
            case State.GamemodePanel:
                // if player selected LOCKED level, redo currentState
                if (LevelSelector.Instance.GetSelectedLevelData().key == "lock")
                {
                    _currentState--;
                }
                break;

            case State.Loading:
                LoadScene();
                break;
        }

        UIMenuManager.Instance.UpdatePanel();
    }

    void HorizontalPressed(float value)
    {
        if (_currentState == State.GamemodePanel)
        {
            UIMenuManager.Instance.SliderGamemodeSettings.value += (int)value;
        }
    }
    #endregion

    void LoadScene()
    {
        // pass the gamemode settings 
        var gamemodeData = _gamemodeData[(int)_selectedGamemode];
        int sliderIndex = (int)UIMenuManager.Instance.SliderGamemodeSettings.value;
        AbstractGamemode.valueForVictory = gamemodeData.ValuesSettings[sliderIndex];

        // display play view on controllers
        if (AirConsole.instance.IsAirConsoleUnityPluginReady())
        {
            var token = new
            {
                view = ControllerView.Play.ToString(),
            };

            AirConsole.instance.Broadcast(token);
        }

        // then, finally load the scene

        string level = "SC_" + LevelSelector.Instance.GetSelectedLevelData().key;

        Debug.Log("Loading level named " + level);
        SceneManager.LoadScene(level);
    }
    #endregion
}
