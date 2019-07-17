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
        string winnerNickname = AirConsole.instance.GetNickname(AirConsole.instance.ConvertPlayerNumberToDeviceId(winnerPlayerNumber));
        _winnerWrapper.GetComponentInChildren<TextMeshProUGUI>().text = winnerNickname;

        int deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(winnerPlayerNumber);
        string url = AirConsole.instance.GetProfilePicture(deviceId, 256);
        _winnerWrapper.GetComponentInChildren<Image>().gameObject.AddComponent<ImageLoader>().url = url;

        _victoryPanel.SetActive(true);
        GameManager.Instance.canRestart = true;
    }
}
