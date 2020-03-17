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
    void Start()
    {
        InitializeSliders();

        GameManager.Instance.Gamemode.OnScoreUpdate += OnScoreUpdate;

        // TODO: maj profiles picturess
        GameManager.Instance.OnCharacterSpawn += SetSliderPicture;
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
        // TODO: maj profiles picturess
        int index = (int)charId;

        // set outline
        _playerPicturesWrapper[index].Outline.color = charId.GetUIColor();

        // set avatar
        int deviceId = CharIdAllocator.GetDeviceId(charId);
        if (deviceId != -1)
        {
            ProfilePictureManager.Instance.SetProfilePicture(deviceId, _playerPicturesWrapper[index].Avatar);
        }
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
        UpdateSliders_SortOrder(_coloredSliders, score); // another pass to avoid sibling errors
    }

    void UpdateSlider_Content(Slider slider, int score)
    {
        slider.DOValue(score, _barAnimationDuration).SetEase(_barAnimationEase);
       
        // TODO: que se passe-t-il quand il y a 2 joueurs au même score ?
    }

    void UpdateSliders_SortOrder(Slider[] sliders, int[] score)
    {
        // order by descending order sliders
        Slider[] orderedSliders = sliders.OrderByDescending(x => x.value).ToArray();

        // then, set their sibling index
        for (int i = 0; i < orderedSliders.Length; i++)
        {
            orderedSliders[i].transform.SetSiblingIndex(i);
        }
    }
    #endregion
    #endregion
}
