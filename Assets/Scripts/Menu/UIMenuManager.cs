using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuManager : Singleton<UIMenuManager>
{
    #region Fields
    [Header("Panel Level Select")]
    [SerializeField] private GameObject _panelLevelSelection;
    [Space]
    [SerializeField] private TextMeshProUGUI _textWaitingForPlayers;
    [SerializeField] private PlayerWrapper[] _playersWrappers = new PlayerWrapper[4];
    [Header("Loading")]
    [SerializeField] private GameObject _panelLoading;
    [Space]
    [SerializeField] private TextMeshProUGUI _textLoading;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        _panelLevelSelection.SetActive(true);
        _panelLoading.SetActive(false);

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
        var deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(playerNumber);

        // activate childs
        _playersWrappers[playerNumber].transform.ActionForEachChildren((GameObject child) =>
        {
            child.SetActive(true);
        });

        // update outline
        _playersWrappers[playerNumber].Outline.effectColor = ((CharID)playerNumber).ToColor();

        // update text
        _playersWrappers[playerNumber].Name.text = AirConsole.instance.GetNickname(deviceId);

        // update image
        ProfilePictureManager.Instance.SetProfilePicture(deviceId, _playersWrappers[playerNumber].Avatar);
    }
    #endregion

    #region Panel Loading
    public void SetActivePanelLoading()
    {
        _textLoading.text = _textLoading.text.Replace("$value$", MenuManager.Instance.SelectedGamemodeDefaultValue.ToString());

        _panelLevelSelection.SetActive(false);
        _panelLoading.SetActive(true);
    }
    #endregion
    #endregion
}

