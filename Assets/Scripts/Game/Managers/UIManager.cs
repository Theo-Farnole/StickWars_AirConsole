using NDream.AirConsole;
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
    [SerializeField] private PlayerWrapper[] _gamemodeData = new PlayerWrapper[GameManager.MAX_PLAYERS];
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

        // hide avatar wrappers
        for (int i = 0; i < GameManager.MAX_PLAYERS; i++)
        {
            _gamemodeData[i].gameObject.SetActive(false);
            //_gamemodeData[i].Name.color = ((CharID)i).ToColor(); // image loader
            Debug.LogWarning("Add image loader");
            _gamemodeData[i].Outline.effectColor = ((CharID)i).ToColor();
        }
    }
    #endregion

    public void SetAvatars()
    {
        var activePlayers = AirConsole.instance.GetActivePlayerDeviceIds.Count;

        for (int i = 0; i < GameManager.MAX_PLAYERS; i++)
        {
            var image = _gamemodeData[i].GetComponentInChildren<Image>();

            if (image != null)
            {
                image.color = Color.white;

                Debug.LogWarning("Commented usefull code block");
                //// add image loader
                //if (image.GetComponent<ImageLoader>() == null)
                //{
                //    int deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(i);
                //    string url = AirConsole.instance.GetProfilePicture(deviceId, 256);
                //    image.gameObject.AddComponent<ImageLoader>().url = url;
                //}
            }

            // active or not wrapper
            bool isPlayerActive = (i < activePlayers);
            _gamemodeData[i].gameObject.SetActive(isPlayerActive);
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
        //_winnerWrapper.GetComponentInChildren<Image>().gameObject.AddComponent<ImageLoader>().url = url;
        Debug.LogWarning("tamer");

        _victoryPanel.SetActive(true);
        this.ExecuteAfterTime(VICTORY_SCREEN_DURATION, () =>
        {
            SceneManager.LoadScene("_SC_menu");
        });
    }
}
