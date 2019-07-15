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

    public void UpdatePlayersAvatar()
    {
        var devices = AirConsole.instance.GetControllerDeviceIds();

        for (int i = 0; i < GameManager.MAX_PLAYERS; i++)
        {

            if (i < devices.Count)
            {
                var deviceId = AirConsole.instance.ConvertPlayerNumberToDeviceId(i);
                DisplayPlayer(i, deviceId);
            }

            else
            {
                // hide childs
                _playersAvatar[i].transform.ActionForEachChildren((GameObject child) =>
                {
                    child.SetActive(false);
                });
            }
        }

        // display text waiting or not
        bool shouldDisplayTextWaiting = (devices.Count == 0);
        _textWaitingForPlayers.gameObject.SetActive(shouldDisplayTextWaiting);
    }

    private void DisplayPlayer(int index, int device_id)
    {
        // update image
        var image = GetComponentInChildren<Image>();

        if (image)
        {
            var url = AirConsole.instance.GetProfilePicture(device_id);
            //WWW www = new WWW(url);

            image.color = ((CharID)index).ToColor();
        }

        // update text
        var text = GetComponentInChildren<TextMeshProUGUI>();

        if (text)
        {
            text.text = AirConsole.instance.GetNickname(device_id);
        }

        // activate childs
        _playersAvatar[index].transform.ActionForEachChildren((GameObject child) =>
        {
            child.SetActive(true);
        });
    }
}
