using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Fields
    [SerializeField] private Canvas _mainCanvas;
    [Header("Game Panel")]
    [SerializeField] private TextMeshProUGUI _textSceneName;
    [Space]
    [SerializeField] private GameObject[] _gamemodeData = new GameObject[GameManager.MAX_PLAYERS];
    [Header("Victory Animation")]
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private TextMeshProUGUI _textVictory;
    [SerializeField] private Image _crown;
    [SerializeField] private GameObject _winnerWrapper;
    #endregion

    #region MonoBehaviour Callbacks
    void Awake()
    {
        _textSceneName.text = SceneManager.GetActiveScene().name;
        _victoryPanel.SetActive(false);
    }

    void Start()
    {
        SetColorsGamemodeData();
    }
    #endregion

    public void SetColorsGamemodeData()
    {
        for (int i = 0; i < GameManager.MAX_PLAYERS; i++)
        {
            _gamemodeData[i].GetComponentInChildren<Image>().color = ((CharID)i).ToColor();

            bool isPlayerActive = (i < AirConsole.instance.GetActivePlayerDeviceIds.Count);
            _gamemodeData[i].SetActive(isPlayerActive);
        }
    }

    public void UpdateGamemodeData(dynamic[] arrayStr)
    {
        for (int i = 0; i < arrayStr.Length && i < GameManager.MAX_PLAYERS; i++)
        {
            _gamemodeData[i].GetComponentInChildren<TextMeshProUGUI>().text = arrayStr[i].ToString();
        }
    }

    public void LaunchVictoryAnimation(int winnerPlayerNumber)
    {
        for (int i = 0; i < GameManager.MAX_PLAYERS; i++)
        {
            if (_gamemodeData[i].activeInHierarchy)
            {
                _gamemodeData[i].GetComponentInChildren<TextMeshProUGUI>().Fade(FadeType.FadeOut, 0.3f);
                _gamemodeData[i].GetComponentInChildren<Image>().Fade(FadeType.FadeOut, 0.3f);
            }

            // copy winner's data into victory wrapper
            if (i == winnerPlayerNumber)
            {
                _winnerWrapper.GetComponentInChildren<TextMeshProUGUI>().text = _gamemodeData[i].GetComponentInChildren<TextMeshProUGUI>().text;
                _winnerWrapper.GetComponentInChildren<Image>().sprite = _gamemodeData[i].GetComponentInChildren<Image>().sprite;
            }
        }

        _victoryPanel.SetActive(true);
        GameManager.Instance.canRestart = true;
    }
}
