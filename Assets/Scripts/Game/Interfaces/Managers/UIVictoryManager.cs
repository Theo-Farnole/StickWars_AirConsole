using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIVictoryManager : Singleton<UIVictoryManager>
{
    public static readonly float VICTORY_SCREEN_DURATION = 1.8f;

    #region Fields
    [Header("COMPONENTS LINKING")]
    [SerializeField] private GameObject _victoryCanvas;
    [SerializeField] private TextMeshProUGUI _textVictory;
    [SerializeField] private Image _crown;
    [SerializeField] private GameObject _winnerWrapper;

    [Header("EVENTS")]
    public UnityEvent OnLaunchVictoryAnimation;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _victoryCanvas.SetActive(false);
    }
    #endregion

    public void LaunchVictoryAnimation(CharId winnerCharId)
    {
        DeactivateGamePanel();

        CameraEffectController.Instance.EnableBlur(true);

        int winnerDeviceId = CharIdAllocator.GetDeviceId(winnerCharId);
        string winnerNickname = AirConsole.instance.GetNickname(winnerDeviceId);
        _winnerWrapper.GetComponentInChildren<TextMeshProUGUI>().text = winnerNickname;

        ProfilePictureManager.Instance.SetProfilePicture(winnerDeviceId, _winnerWrapper.GetComponentInChildren<Image>());

        _victoryCanvas.SetActive(true);
        this.ExecuteAfterTime(VICTORY_SCREEN_DURATION, () =>
        {
            AirConsole.instance.ShowAd();
            AirConsole.instance.onAdComplete += (bool adWasShown) => SceneManager.LoadScene("_SC_menu");
        });
    }

    void DeactivateGamePanel()
    {
        // TODO: deactive tuto textb o
        var tutorialTextBoxManager = FindObjectOfType<TutorialTextBoxManager>();
        tutorialTextBoxManager.TextBoxCanvas.gameObject.SetActive(false);

        // TODO: deactie goal progress bar
        var goalBarManager = FindObjectOfType<GoalBarManager>();
        goalBarManager.CanvasGoalBarManager.gameObject.SetActive(false);
    }
    #endregion
}
