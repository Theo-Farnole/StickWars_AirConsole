using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public readonly static float CHANGE_LEVEL_TIME_DELAY = 0.3f;

    #region Fields
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
    #endregion

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
            SceneManager.LoadScene("SC_Windows");
            //SceneManager.LoadScene("SC_" + LevelSelector.Instance.GetSelectedLevelData().key);
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

    void OnMessage(int device_id, JToken data)
    {
        if (device_id == AirConsole.instance.GetMasterControllerDeviceId())
        {
            if (_canChangeLevel && data["horizontal"] != null)
            {
                LevelSelector.Instance.SelectedLevel += (int)data["horizontal"];

                CanChangeLevel = false;
            }

            if (data["aPressed"] != null && (bool)data["aPressed"])
            {
                SceneManager.LoadScene("SC_Windows");
                //SceneManager.LoadScene("SC_" + LevelSelector.Instance.GetSelectedLevelData().key);

                var token = new
                {
                    view = ControllerView.Play.ToString(),
                };

                AirConsole.instance.Broadcast(token);
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

            if (AirConsole.instance.GetControllerDeviceIds().Count > GameManager.MAX_PLAYERS)
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

            Debug.Log(bgColor + " view = " + view);

            var token = new
            {
                view,
                bgColor
            };

            AirConsole.instance.Message(device_id, token);
        }
    }
}
