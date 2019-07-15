using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuManager : Singleton<UIMenuManager>
{
    #region Fields
    [SerializeField] private GameObject[] _playersAvatar = new GameObject[4];
    [Space]
    [SerializeField] private TextMeshProUGUI _textWaitingForPlayers;
    #endregion

    #region MonoBehaviour Callbacks
    void Start()
    {
        if (!AirConsole.instance.IsAirConsoleUnityPluginReady())
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
    }
    #endregion

    public void UpdatePlayersAvatar()
    {
        var devices = AirConsole.instance.GetControllerDeviceIds();
        
        for (int i = 0; i < GameManager.MAX_PLAYERS; i++)
        {

            if (i < devices.Count)
            {
                DisplayPlayerAvatar(i);
            }
            else
            {
                // hide childs
                _playersAvatar[i].transform.ActionForEachChildren((GameObject child) =>
                {
                    child.SetActive(false);
                });
            }
        }

        // display text waiting or not
        bool shouldDisplayTextWaiting = (devices.Count == 0);
        _textWaitingForPlayers.gameObject.SetActive(shouldDisplayTextWaiting);
    }

    private void DisplayPlayerAvatar(int index)
    {
        // activate childs
        _playersAvatar[index].transform.ActionForEachChildren((GameObject child) =>
        {
            child.SetActive(true);
        });

        // update image
        var image = _playersAvatar[index].GetComponentInChildren<Image>();

        if (image)
        {
            image.color = ((CharID)index).ToColor();
        }

        // update text
        var text = _playersAvatar[index].GetComponentInChildren<TextMeshProUGUI>();

        if (text)
        {
            var deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(index);
            text.text = AirConsole.instance.GetNickname(deviceId);
        }
    }
}
