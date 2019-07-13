using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuManager : Singleton<UIMenuManager>
{
    #region Fields
    [SerializeField] private GameObject[] _playersAvatar = new GameObject[4];
    [Space]
    [SerializeField] private TextMeshProUGUI _textWaitingForPlayers;
    #endregion

    #region MonoBehaviour Callbacks
    void Start()
    {
        // hide avatar until player join
        for (int i = 0; i < _playersAvatar.Length; i++)
        {
            _playersAvatar[i].transform.ActionForEachChildren((GameObject c) =>
            {
                c.SetActive(false);
            });
        }
    }
    #endregion

    // TODO: Optimize this method!
    public void DisplayPlayer(int index, int device_id)
    {
        _textWaitingForPlayers.gameObject.SetActive(false);

        _playersAvatar[index].transform.ActionForEachChildren((GameObject c) =>
        {
            c.SetActive(true);

            var image = c.GetComponent<Image>();

            if (image)
            {
                image.color = ((CharID)index).ToColor();
            }

            var text = c.GetComponent<TextMeshProUGUI>();

            if (text)
            {
                text.text = AirConsole.instance.GetNickname(device_id);
            }
        });
    }
}
