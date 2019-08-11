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
    [Header("Panel Players & Map")]
    [SerializeField] private GameObject _panelPlayers;
    [Space]
    [SerializeField] private TextMeshProUGUI _textWaitingForPlayers;
    [SerializeField] private PlayerWrapper[] _playersWrappers = new PlayerWrapper[4];
    [Header("Panel Gamemode")]
    [SerializeField] private GameObject _panelGamemode;
    [Space]
    [SerializeField] private Slider _sliderGamemodeSettings;
    [SerializeField] private TextMeshProUGUI[] _textValuesSlider = new TextMeshProUGUI[5];
    #endregion

    #region Properties
    public Slider SliderGamemodeSettings { get => _sliderGamemodeSettings;}
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        UpdatePanel();        

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

    #region Panel Players & Map
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

    #region Panel Gamemode
    void UpdateSliderValues()
    {
        var values = MenuManager.Instance.GamemodeData[(int)GamemodeType.DeathMatch].ValuesSettings;

        for (int i = 0; i < _textValuesSlider.Length; i++)
        {
            _textValuesSlider[i].text = values[i].ToString();
        }

    }
    #endregion

    public void UpdatePanel()
    {
        _panelPlayers.SetActive(false);
        _panelGamemode.SetActive(false);

        switch (MenuManager.Instance.CurrentState)
        {
            case MenuManager.State.PlayerPanel:
                _panelPlayers.SetActive(true);
                break;

            case MenuManager.State.GamemodePanel:
                UpdateSliderValues();
                _panelGamemode.SetActive(true);
                break;
        }
    }
    #endregion
}

