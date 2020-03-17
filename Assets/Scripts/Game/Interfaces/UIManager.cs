using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public static readonly float VICTORY_SCREEN_DURATION = 1.8f;

    #region Fields
    [SerializeField] private Canvas _mainCanvas;
    [Header("Game Panel")]
    [SerializeField] private GameObject _gamePanel;
    [Header("Victory Animation")]
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private TextMeshProUGUI _textVictory;
    [SerializeField] private Image _crown;
    [SerializeField] private GameObject _winnerWrapper;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _victoryPanel.SetActive(false);
    }
    #endregion

    public void LaunchVictoryAnimation(CharId winnerCharId)
    {
        _gamePanel.SetActive(false);
        CameraEffectController.Instance.EnableBlur(true);

        int winnerDeviceId = CharIdAllocator.GetDeviceId(winnerCharId);
        string winnerNickname = AirConsole.instance.GetNickname(winnerDeviceId);
        _winnerWrapper.GetComponentInChildren<TextMeshProUGUI>().text = winnerNickname;

        ProfilePictureManager.Instance.SetProfilePicture(winnerDeviceId, _winnerWrapper.GetComponentInChildren<Image>());

        _victoryPanel.SetActive(true);
        this.ExecuteAfterTime(VICTORY_SCREEN_DURATION, () =>
        {
            AirConsole.instance.ShowAd();
            AirConsole.instance.onAdComplete += (bool adWasShown) => SceneManager.LoadScene("_SC_menu");
        });
    }
    #endregion
}
