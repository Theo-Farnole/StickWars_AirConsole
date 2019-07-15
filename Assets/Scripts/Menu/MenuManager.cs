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
        AirConsole.instance.onReady += OnReady;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            SceneManager.LoadScene("SC_" + LevelSelector.Instance.GetSelectedLevelData().key);
        }
    }
#endif 

    void OnDestroy()
    {
        //AirConsole.instance.onMessage -= OnMessage;
        AirConsole.instance.onConnect -= OnConnect;
        AirConsole.instance.onDisconnect -= OnDisconnect;
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
                SceneManager.LoadScene("SC_" + LevelSelector.Instance.GetSelectedLevelData().key);
            }
        }
    }

    void OnConnect(int device_id)
    {
        UIMenuManager.Instance.UpdatePlayersAvatar();
    }

    void OnReady(string str)
    {
        UIMenuManager.Instance.UpdatePlayersAvatar();
    }

    void OnDisconnect(int device_id)
    {
        var devices = AirConsole.instance.GetControllerDeviceIds();
        UIMenuManager.Instance.UpdatePlayersAvatar();
    }
}
