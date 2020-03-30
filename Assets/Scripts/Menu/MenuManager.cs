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
    [SerializeField] private AllGamemodesData _allGamemodesData;

    private GamemodeType _selectedGamemode = GamemodeType.DeathMatch;
    #endregion

    #region Properties
    public int SelectedGamemodeDefaultValue { get => _allGamemodesData.GetGamemodeValue(_selectedGamemode); }
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
        CharIdAllocator.AllocateDeviceId(deviceId);

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
        CharIdAllocator.DeallocateDeviceId(device_id);

        UIMenuManager.Instance.UpdatePlayersAvatar();
        UpdateControllerView();
    }
    #endregion

    void UpdateControllerView()
    {
        List<int> devices = AirConsole.instance.GetControllerDeviceIds();

        for (int i = 0; i < devices.Count; i++)
        {
            int deviceId = devices[i];
            CharId? charId = CharIdAllocator.GetCharId(deviceId);

            // if not charId allocated
            if (charId == null)
            {
                var token2 = new
                {
                    view = ControllerView.NoPlace.ToString()
                };

                AirConsole.instance.Message(deviceId, token2);
                continue;
            }
            else
            {
                string view = string.Empty;

                if (deviceId == AirConsole.instance.GetMasterControllerDeviceId())
                {
                    view = ControllerView.Menu.ToString();
                }
                else
                {
                    view = ControllerView.Wait.ToString();
                }

                var token = new
                {
                    view,
                    charId = ((CharId)charId).ToString(),
                    bgColor = ((CharId)charId).GetUIHex()
                };

                AirConsole.instance.Message(deviceId, token);
            }
        }
    }

    #region Handle Input
    void APressed()
    {
        if (CharIdAllocator.AllocatedCharIdCount < 2)
        {
            UIMenuManager.Instance.DisplayNoEnoughPlayersText(true);
        }
        else
        {
            UIMenuManager.Instance.SetActivePanelLoading();
            this.ExecuteAfterTime(LOADING_TIME, LoadScene);

            GetComponentInChildren<AudioSource>()?.Play();
        }

    }
    #endregion

    void LoadScene()
    {
        // pass the gamemode settings         
        AbstractGamemode.valueForVictory = SelectedGamemodeDefaultValue;

        // display play view on controllers
        var token = new
        {
            view = ControllerView.Load.ToString(),
        };

        AirConsole.instance.Broadcast(token);

        // then, finally load the scene
        SceneManager.LoadScene("SC_Level_1");
    }
    #endregion
}
