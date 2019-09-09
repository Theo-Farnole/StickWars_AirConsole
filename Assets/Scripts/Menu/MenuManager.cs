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
    public static readonly float LOADING_TIME = 1.5f;

    #region Fields
    [EnumNamedArray(typeof(GamemodeType))]
    [SerializeField] private GamemodeData[] _gamemodeData = new GamemodeData[Enum.GetValues(typeof(GamemodeType)).Length];

    private GamemodeType _selectedGamemode = GamemodeType.DeathMatch;
    #endregion

    #region Properties
    public GamemodeData[] GamemodeData { get => _gamemodeData; }
    public int SelectedGamemodeDefaultValue
    {
        get
        {
            return _gamemodeData[(int)_selectedGamemode].DefaultValue;
        }
    }
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
            if (data["aPressed"] != null && (bool)data["aPressed"])
            {
                APressed();
            }
        }
    }

    void OnConnect(int deviceId)
    {
        Debug.Log("Onconnect");

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

            string bgColor = ((CharID)AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id)).GetUIHex();
            string charId = ((CharID)AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id)).ToString();
            string view = string.Empty;

            if (device_id == AirConsole.instance.GetMasterControllerDeviceId())
            {
                view = ControllerView.Menu.ToString();
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
                charId,
                bgColor
            };

            AirConsole.instance.Message(device_id, token);
        }
    }

    #region Handle Input
    void APressed()
    {
        if (LevelSelector.Instance.GetSelectedLevelData().key != "lock")
        {
            UIMenuManager.Instance.SetActivePanelLoading();
            this.ExecuteAfterTime(LOADING_TIME, LoadScene);
        }
    }
    #endregion

    void LoadScene()
    {
        // pass the gamemode settings 
        AbstractGamemode.valueForVictory = SelectedGamemodeDefaultValue;

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
