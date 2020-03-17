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
    [SerializeField, EnumNamedArray(typeof(CharId))] private Slider[] _coloredSliders;
    [SerializeField, EnumNamedArray(typeof(CharId))] private Slider[] _picturesSliders;
    [Space]
    [SerializeField, EnumNamedArray(typeof(CharId))] private PlayerWrapper[] _playerPicturesWrapper;
    [Header("Animation")]
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
        InitializeSliders();

        // reactive bar on spawn
        GameManager.Instance.Gamemode.OnScoreUpdate += OnScoreUpdate;
        GameManager.Instance.OnCharacterSpawn += SetSliderPicture;

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

        string o = string.Empty;

        foreach (var item in Enum.GetValues(typeof(CharId)))
        {
            o += item + " " + score[(int)item] + "\n";
        }

        Debug.Log(o);
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
    }

    void UpdateSlider_Content(Slider slider, int score)
    {
        slider.DOValue(score, _barAnimationDuration).SetEase(_barAnimationEase);

        // TODO: que se passe-t-il quand il y a 2 joueurs au même score ?
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
    #endregion
    #endregion
}
