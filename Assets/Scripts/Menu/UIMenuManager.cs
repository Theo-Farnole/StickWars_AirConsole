using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuManager : Singleton<UIMenuManager>
{
    #region Fields
    [SerializeField] private PlayerWrapper[] _playersWrappers = new PlayerWrapper[4];
    [Space]
    [SerializeField] private TextMeshProUGUI _textWaitingForPlayers;
    #endregion

    #region MonoBehaviour Callbacks
    void Start()
    {
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

    public void UpdatePlayersAvatar()
    {
        var devices = AirConsole.instance.GetActivePlayerDeviceIds;

        for (int i = 0; i < GameManager.MAX_PLAYERS; i++)
        {
            if (i < devices.Count)
            {
                DisplayPlayerAvatar(i);
            }
            else
            {
                // hide childs
                _playersWrappers[i].transform.ActionForEachChildren((GameObject child) =>
                {
                    child.SetActive(false);
                });
            }
        }

        // display text waiting or not
        bool shouldDisplayTextWaiting = (devices.Count == 0);
        _textWaitingForPlayers.gameObject.SetActive(shouldDisplayTextWaiting);
    }

    private void DisplayPlayerAvatar(int playerNumber)
    {
        // activate childs
        _playersWrappers[playerNumber].transform.ActionForEachChildren((GameObject child) =>
        {
            child.SetActive(true);
        });

        // update image
        Debug.LogWarning("Add ImageLoader here");
        //_playersWrappers[index].Avatar.sprite = ((CharID)index).ToColor();

        // update outline
        _playersWrappers[playerNumber].Outline.effectColor = ((CharID)playerNumber).ToColor();

        // update text
        var deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(playerNumber);
        _playersWrappers[playerNumber].Name.text = AirConsole.instance.GetNickname(deviceId);
    }
}

