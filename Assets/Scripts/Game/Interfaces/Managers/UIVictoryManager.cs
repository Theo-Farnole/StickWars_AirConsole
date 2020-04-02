using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable] public class UnityEventFloat : UnityEvent<float> { }

public class UIVictoryManager : Singleton<UIVictoryManager>
{
    public static readonly float VICTORY_SCREEN_DURATION = 4.5f;
    public static readonly float ANIMATION_DURATION = 0.8f;
    public static readonly float DELAY_BETWEEN_ANIMATION = 0.2f;

    #region Fields
    [Header("COMPONENTS LINKING")]
    [SerializeField] private GameObject _victoryCanvas;
    [SerializeField] private TextMeshProUGUI _textVictory;
    [SerializeField] private RemainingTimeVictory _timerRemainingTime;
    [Space]
    [SerializeField] private TextMeshProUGUI _winnerNickname;
    [SerializeField] private Image _winnerProfilePicture;
    [SerializeField] private Image _winnerProfilePictureOutline;
    [Space]
    [SerializeField] private UI_SpecialPlayerWrapper[] _specialPlayerWrappers;

    [Header("DEBUG")]
    [SerializeField] private bool _dontShowAdsInEditor = true;

    [Header("EVENTS")]
    public UnityEventFloat OnLaunchVictoryAnimation;
    #endregion

    #region Properties
    public float SpecialTitleAnimationDuration
    {
        get
        {
            int activePlayerCount = GameManager.Instance.InstantiatedCharactersCount - 1;
            float sumAnimationDuration = activePlayerCount * (DELAY_BETWEEN_ANIMATION + ANIMATION_DURATION);

            return sumAnimationDuration;
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _victoryCanvas.SetActive(false);

        for (int i = 0; i < _specialPlayerWrappers.Length; i++)
        {
            _specialPlayerWrappers[i].gameObject.SetActive(false);
        }
    }
    #endregion

    public void LaunchVictoryAnimation(CharId winnerCharId)
    {
        // update canvas activation
        DeactivateGamePanel();
        _victoryCanvas.SetActive(true);

        OnLaunchVictoryAnimation?.Invoke(VICTORY_SCREEN_DURATION);

        CameraEffectController.Instance.EnableBlur(true);

        // update content
        SetWinnerContent(winnerCharId);
        SetSpecialPlayersContent(winnerCharId);

        // setup delayed tasks
        float cachedAnimationDuration = SpecialTitleAnimationDuration;

        // start timer
        this.ExecuteAfterTime(cachedAnimationDuration, () =>
        {
            _timerRemainingTime.StartTimer(VICTORY_SCREEN_DURATION);
        });

        // play ad
        (this).ExecuteAfterTime(cachedAnimationDuration + VICTORY_SCREEN_DURATION, () =>
        {
            ShowAdThenShowScene();
        });
    }

    private void ShowAdThenShowScene()
    {
#if UNITY_EDITOR
        if (_dontShowAdsInEditor)
        {
            SceneManager.LoadScene("SC_Menu_Main");
            return;
        }
#endif

        AirConsole.instance.ShowAd();
        AirConsole.instance.onAdComplete += (bool adWasShown) => SceneManager.LoadScene("SC_Menu_Main");
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

        var characterStatistics = FindObjectOfType<StatisticsManager>().CharacterStatistics;
        Dictionary<CharId, string> titles = GetSpecialTitle(winnerCharId, characterStatistics);

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

            wrapper.gameObject.SetActive(false);

            // content
            wrapper.UpdateCharIdContent(charId);
            wrapper.SpecialTitle.text = titles[charId];

            // animation
            float animationDelay = i * DELAY_BETWEEN_ANIMATION + i * ANIMATION_DURATION;

            this.ExecuteAfterTime(animationDelay, () => wrapper.gameObject.SetActive(true));

            wrapper.transform.localScale = Vector3.one * 1.4f;
            wrapper.transform.DOScale(Vector3.one, ANIMATION_DURATION)
                .SetEase(Ease.InQuint)
                .SetDelay(animationDelay)
                .OnComplete(() => CameraShake.Instance.Shake(0.1f, 0.05f));
        }
    }

    #region Special title attributions
    Dictionary<CharId, string> GetSpecialTitle(CharId winnerId, Dictionary<CharId, CharacterStatistics> characterStatistics)
    {
        Dictionary<CharId, string> specialTitles = new Dictionary<CharId, string>();

        characterStatistics.Remove(winnerId);

        CheckSpecialTitle(ref characterStatistics, ref specialTitles, "_jumpCount", 0, "Climber");
        CheckSpecialTitle(ref characterStatistics, ref specialTitles, "_virusReleased", 1, "Hacker");
        CheckSpecialTitle(ref characterStatistics, ref specialTitles, "_tackleSumDamage", 80, "Scrapper");
        CheckSpecialTitle(ref characterStatistics, ref specialTitles, "_projectileThrowCount", 4, "Crazy Shooter");

        // fill empty title by a random title
        foreach (var charId in (CharId[])Enum.GetValues(typeof(CharId)))
        {
            if (!specialTitles.ContainsKey(charId))
            {
                specialTitles.Add(charId, "Collector");
            }
        }

        return specialTitles;
    }

    void CheckSpecialTitle(ref Dictionary<CharId, CharacterStatistics> characterStatistics, ref Dictionary<CharId, string> specialTitles, string fieldToCompare, int condition, string title)
    {
        FieldInfo field = typeof(CharacterStatistics).GetField(fieldToCompare, BindingFlags.NonPublic | BindingFlags.Instance);

        var firstOfSpecialTitle = characterStatistics
                                        .Select(x => x.Value)
                                        .Where(x => (int)field.GetValue(x) >= condition)  // achieve condition
                                        .OrderBy(x => (int)field.GetValue(x)) // the first, the better
                                        .FirstOrDefault();

        if (firstOfSpecialTitle != null)
        {
            // get key from value
            var charId = characterStatistics.FirstOrDefault(x => x.Value == firstOfSpecialTitle).Key;

            // set title to the first of special title
            specialTitles.Add(charId, title);
        }
    }
    #endregion
    #endregion
}
