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
    [Header("Players' Avatar")]
    [SerializeField] private GameObject[] _playersAvatar = new GameObject[4];

    private bool _canChangeLevel = true;
    #endregion

    #region MonoBehaviour Callbacks
    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    void Start()
    {
        // hide avatar until player join
        for (int i = 0; i < _playersAvatar.Length; i++)
        {
            _playersAvatar[i].transform.ActionForEachChildren((GameObject c) =>
            {
                c.SetActive(false);
            });
        }
    }

    void OnDestroy()
    {
        AirConsole.instance.onMessage -= OnMessage;
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

                _canChangeLevel = false;

                this.ExecuteAfterTime(CHANGE_LEVEL_TIME_DELAY, () =>
                {
                    _canChangeLevel = true;
                });
            }

            if (data["aPressed"] != null && (bool)data["aPressed"])
            {
                SceneManager.LoadScene("SC_" + LevelSelector.Instance.GetSelectedLevelData().key);
            }
        }
    }

    // TODO: Optimize this method!
    void OnConnect(int device_id)
    {
        Debug.Log("On connect!");

        var playerAvatarIndex = AirConsole.instance.GetControllerDeviceIds().Count - 1;

        Debug.Log("playerAvatarIndex " + playerAvatarIndex);

        _playersAvatar[playerAvatarIndex].transform.ActionForEachChildren((GameObject c) =>
        {
            c.SetActive(true);

            var image = c.GetComponent<Image>();

            if (image)
            {
                image.color = ((CharID)playerAvatarIndex).ToColor();
            }

            var text = c.GetComponent<TextMeshProUGUI>();

            if (text)
            {
                text.text = AirConsole.instance.GetNickname(device_id);
            }
        });
    }

    void OnDisconnect(int device_id)
    {
        Debug.LogError("OnDisconnect not implemented");
    }

}
