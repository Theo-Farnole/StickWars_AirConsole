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
    [SerializeField] private TextMeshProUGUI _textSceneName;
    [Space]
    [SerializeField] private PlayerWrapper[] _playersWrappers = new PlayerWrapper[GameManager.MAX_PLAYERS];
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
        _textSceneName.text = SceneManager.GetActiveScene().name;
        _victoryPanel.SetActive(false);

        // hide avatar wrappers
       for (int i = 0; i < _playersWrappers.Length; i++)
        {
            _playersWrappers[i].gameObject.SetActive(false);
        }
    }
    #endregion

    public void SetAvatars()
    {
        foreach (CharID item in Enum.GetValues(typeof(CharID)))
        {
            int i = (int)item;
            // active or not wrapper
            bool isPlayerActive = (GameManager.Instance.Characters.ContainsKey(item) && GameManager.Instance.Characters[item] != null);
            _playersWrappers[i].gameObject.SetActive(isPlayerActive);

            Debug.Log(item + " is active: " + isPlayerActive);

            // load avatar
            if (isPlayerActive)
            {
                _playersWrappers[i].Outline.effectColor = ((CharID)i).GetUIColor();
                int deviceId = GameManager.Instance.CharControllerToDeviceID[item];

                if (deviceId != -1)
                {
                    ProfilePictureManager.Instance.SetProfilePicture(deviceId, _playersWrappers[i].Avatar);
                }

            }
        }
    }

    public void UpdateGamemodeData(int[] arrayStr)
    {
        for (int i = 0; i < arrayStr.Length && i < _playersWrappers.Length; i++)
        {
            _playersWrappers[i].GetComponentInChildren<TextMeshProUGUI>().text = arrayStr[i].ToString();
        }
    }

    public void LaunchVictoryAnimation(CharID winnerCharId)
    {
        int winnerDeviceId = GameManager.Instance.CharControllerToDeviceID[winnerCharId];
        CameraEffectController.Instance.EnableBlur(true);

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
