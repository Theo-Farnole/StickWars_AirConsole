using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIVictoryManager : Singleton<UIVictoryManager>
{
    public static readonly float VICTORY_SCREEN_DURATION = 4.5f;

    #region Fields
    [Header("COMPONENTS LINKING")]
    [SerializeField] private GameObject _victoryCanvas;
    [SerializeField] private TextMeshProUGUI _textVictory;
    [Space]
    [SerializeField] private TextMeshProUGUI _winnerNickname;
    [SerializeField] private Image _winnerProfilePicture;
    [SerializeField] private Image _winnerProfilePictureOutline;
    [Space]
    [SerializeField] private UI_SpecialPlayerWrapper[] _specialPlayerWrappers;

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
        _victoryCanvas.SetActive(true);

        CameraEffectController.Instance.EnableBlur(true);
        SetWinnerContent(winnerCharId);

        SetSpecialPlayersContent(winnerCharId);

        this.ExecuteAfterTime(VICTORY_SCREEN_DURATION, () =>
        {
            AirConsole.instance.ShowAd();
            AirConsole.instance.onAdComplete += (bool adWasShown) => SceneManager.LoadScene("_SC_menu");
        });
    }

    void DeactivateGamePanel()
    {
        // deactive tutorial text box manager
        var tutorialTextBoxManager = FindObjectOfType<TutorialTextBoxManager>();
        tutorialTextBoxManager.TextBoxCanvas.gameObject.SetActive(false);

        // deactive goal bar manager
        var goalBarManager = FindObjectOfType<GoalBarManager>();
        goalBarManager.CanvasGoalBarManager.gameObject.SetActive(false);
    }

    void SetWinnerContent(CharId winnerCharId)
    {
        Color winnerUIColor = winnerCharId.GetUIColor();

        // update nickname
        _winnerNickname.text = CharIdAllocator.GetNickname(winnerCharId);
        //_winnerNickname.color = winnerUIColor;

        // update profile picture
        _winnerProfilePictureOutline.color = winnerUIColor;
        ProfilePictureManager.Instance.SetProfilePicture(winnerCharId, _winnerProfilePicture);
    }

    void SetSpecialPlayersContent(CharId winnerCharId)
    {
        var charIdsOrderByScore = GameManager.Instance.Gamemode.GetCharIdsOrderByScore();

        Assert.AreEqual(_specialPlayerWrappers.Length, Enum.GetValues(typeof(CharId)).Length - 1, "Special players wrapper should have " + (Enum.GetValues(typeof(CharId)).Length - 1) + " elements!");

        for (int i = 0; i < _specialPlayerWrappers.Length; i++)
        {
            var wrapper = _specialPlayerWrappers[i];
            var charId = charIdsOrderByScore[i + 1]; // we add 1 to skip the winner
           
            // if player is not connected
            if (!CharIdAllocator.IsCharIdConnected(charId))
            {
                // disable wrapper
                wrapper.gameObject.SetActive(false);
                continue;
            }

            wrapper.UpdateCharIdContent(charId);
            // TODO: Set special title
        }
    }
    #endregion
}
