using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI _textSceneName;
    [Space]
    [SerializeField] private GameObject[] _gamemodeData = new GameObject[GameManager.MAX_PLAYERS];

    void Awake()
    {
        _textSceneName.text = SceneManager.GetActiveScene().name;
    }

    void Start()
    {
        SetColorsGamemodeData();
    }

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
}
