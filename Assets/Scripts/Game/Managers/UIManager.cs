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
    [SerializeField] private PlayerWrapper[] _playersWrappers = new PlayerWrapper[GameManager.MAX_PLAYERS];
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
        for (int i = 0; i < _playersWrappers.Length; i++)
        {
            _playersWrappers[i].gameObject.SetActive(false);
            //_gamemodeData[i].Name.color = ((CharID)i).ToColor(); // image loader
            Debug.LogWarning("Add image loader");
            _playersWrappers[i].Outline.effectColor = ((CharID)i).ToColor();
        }
    }
    #endregion

    public void SetAvatars()
    {
        var activePlayers = AirConsole.instance.GetActivePlayerDeviceIds.Count;

        for (int i = 0; i < _playersWrappers.Length; i++)
        {
            // active or not wrapper
            bool isPlayerActive = (i < activePlayers);
            _playersWrappers[i].gameObject.SetActive(isPlayerActive);

            // load avatar
            if (isPlayerActive)
            {
                int deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(i);

                string url = AirConsole.instance.GetProfilePicture(deviceId, 256);
                var imageLoader = _playersWrappers[i].Avatar.gameObject.GetComponent<ImageLoader>();

                // reload image loader
                if (!imageLoader || (imageLoader && imageLoader.url != url))
                {
                    if (imageLoader)
                    {
                        Destroy(imageLoader);
                        _playersWrappers[i].Avatar.sprite = null;
                    }

                    _playersWrappers[i].Avatar.gameObject.AddComponent<ImageLoader>().url = url;
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

    public void LaunchVictoryAnimation(int winnerPlayerNumber)
    {
        string winnerNickname = AirConsole.instance.GetNickname(AirConsole.instance.ConvertPlayerNumberToDeviceId(winnerPlayerNumber));
        _winnerWrapper.GetComponentInChildren<TextMeshProUGUI>().text = winnerNickname;

        int deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(winnerPlayerNumber);
        string url = AirConsole.instance.GetProfilePicture(deviceId, 256);
        _winnerWrapper.GetComponentInChildren<Image>().gameObject.AddComponent<ImageLoader>().url = url;
        Debug.LogWarning("tamer");

        _victoryPanel.SetActive(true);
        this.ExecuteAfterTime(VICTORY_SCREEN_DURATION, () =>
        {
            SceneManager.LoadScene("_SC_menu");
        });
    }
}
