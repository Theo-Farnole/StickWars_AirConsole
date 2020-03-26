using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GoalBarManager : MonoBehaviour
{
    #region Fields
    [Header("LINKING")]
    [SerializeField, EnumNamedArray(typeof(CharId))] private Slider[] _coloredSliders;
    [SerializeField, EnumNamedArray(typeof(CharId))] private Slider[] _picturesSliders;    
    [SerializeField, EnumNamedArray(typeof(CharId))] private PlayerWrapper[] _playerPicturesWrapper;
    [SerializeField] private RectTransform _goalSegmentsParent;
    [Header("CONCURRENCY")]
    [SerializeField] private float _avatarOffsetOnConcurrency = 10;
    [Header("ANIMATION")]
    [SerializeField] private float _barAnimationDuration = 1f;
    [SerializeField] private Ease _barAnimationEase = Ease.OutCubic;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        // deactivate bar
        foreach (var item in (CharId[])Enum.GetValues(typeof(CharId)))
        {
            ActivateProgressBar(item, false);
        }
    }

    void Start()
    {
        InitializeSegments();
        InitializeSliders();

        // reactive bar on spawn
        GameManager.Instance.Gamemode.OnScoreUpdate += OnScoreUpdate;
        GameManager.Instance.OnCharacterSpawn += SetSliderPicture;
        GameManager.Instance.OnCharacterSpawn += 
            (CharController charController) => UpdateSliders_PicturesPosition();

        // if GameManager's Start() is called before this Start(), 
        // set slider picture on each character spawned
        foreach (var kvp in GameManager.Instance.Characters)
        {
            if (kvp.Value != null)
            {
                SetSliderPicture(kvp.Value);
            }
        }
    }
    #endregion

    #region Initialization
    void InitializeSegments()
    {
        int maxScore = GameManager.Instance.Gamemode.ValueForVictory;

        int segmentsToDestroy = _goalSegmentsParent.childCount - maxScore;

        // destroy segment
        for (int i = 0; i < segmentsToDestroy; i++)
        {
            GameObject segment = _goalSegmentsParent.GetChild(i).gameObject;
            Destroy(segment);
        }
    }

    void InitializeSliders()
    {
        int scoreToWin = GameManager.Instance.Gamemode.ValueForVictory;

        foreach (var slider in _coloredSliders)
        {
            slider.minValue = 0;
            slider.maxValue = scoreToWin;

            slider.value = 0;
            slider.interactable = false;
        }

        foreach (var slider in _picturesSliders)
        {
            slider.minValue = 0;
            slider.maxValue = scoreToWin;

            slider.value = 0;
            slider.interactable = false;
        }
    }

    void SetSliderPicture(CharController charController)
    {
        SetSliderPicture(charController.charId);
    }

    void SetSliderPicture(CharId charId)
    {
        int index = (int)charId;

        // set outline
        _playerPicturesWrapper[index].gameObject.SetActive(true);

        // set avatar
        int deviceId = CharIdAllocator.GetDeviceId(charId);
        if (deviceId != -1)
        {
            ProfilePictureManager.Instance.SetProfilePicture(deviceId, _playerPicturesWrapper[index].Avatar);
        }

        // activate progress bar
        ActivateProgressBar(charId, true);
    }

    void ActivateProgressBar(CharId charId, bool active)
    {
        int index = (int)charId;

        _coloredSliders[index].gameObject.SetActive(active);
        _picturesSliders[index].gameObject.SetActive(active);
    }
    #endregion

    #region Events listeners
    void OnScoreUpdate(int[] score, int scoreForVictory)
    {
        UpdateSliders(score);
    }
    #endregion

    #region Update data methods
    void UpdateSliders(int[] score)
    {
        for (int i = 0; i < _coloredSliders.Length; i++)
            UpdateSlider_Content(_coloredSliders[i], score[i]);

        for (int i = 0; i < _picturesSliders.Length; i++)
            UpdateSlider_Content(_picturesSliders[i], score[i]);

        UpdateSliders_SortOrder(_coloredSliders, score);
        UpdateSliders_PicturesPosition();
    }

    void UpdateSlider_Content(Slider slider, int score)
    {
        slider.DOValue(score, _barAnimationDuration).SetEase(_barAnimationEase);
    }

    void UpdateSliders_SortOrder(Slider[] sliders, int[] score)
    {
        KeyValuePair<CharId, int>[] scoreByInt = new KeyValuePair<CharId, int>[4];

        // fill scoreByInt
        for (int i = 0; i < scoreByInt.Length; i++)
        {
            CharId charId = (CharId)i;

            scoreByInt[i] = new KeyValuePair<CharId, int>(charId, score[i]);
        }

        // order scoreByInt
        var orderedScoreByInt = scoreByInt.OrderBy(x => x.Value).ToArray();

        // set sibling index
        for (int i = 0; i < orderedScoreByInt.Length; i++)
        {
            // charId to index
            int sliderIndex = (int)orderedScoreByInt[i].Key;

            sliders[sliderIndex].transform.SetSiblingIndex(i);
        }

        // reverse sliders order
        var slidersParent = sliders[0].transform.parent;
        int childCount = slidersParent.childCount;

        for (int i = 0; i < childCount; i++)
        {
            slidersParent.GetChild(0).SetSiblingIndex((childCount - 1) - i);
        }
    }

    void UpdateSliders_PicturesPosition()
    {
        var charIds = (CharId[])Enum.GetValues(typeof(CharId));

        for (int i = 0; i < _playerPicturesWrapper.Length; i++)
        {
            var charId = charIds[i];

            var rectTransform = _playerPicturesWrapper[i].GetComponent<RectTransform>();

            var index = GameManager.Instance.Gamemode.GetPositionInPlayersAtScore(charId);
            var newPosition = index * _avatarOffsetOnConcurrency * -1;

            rectTransform.DOAnchorPosY(newPosition, _barAnimationDuration).SetEase(Ease.InOutSine);
        }
    }
    #endregion
    #endregion
}
