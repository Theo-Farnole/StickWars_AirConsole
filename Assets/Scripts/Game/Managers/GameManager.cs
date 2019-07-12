using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject _prefabPlayer;

    private CharController[] _characters = new CharController[4];
    #endregion

    #region MonoBehaviour Callbacks
#if !DISABLE_AIRCONSOLE
    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void Start()
    {
        var devicesIds = AirConsole.instance.GetControllerDeviceIds();
        AirConsole.instance.SetActivePlayers(devicesIds.Count);

        for (int i = 0; i < devicesIds.Count; i++)
        {
            int playerNumber = AirConsole.instance.ConvertDeviceIdToPlayerNumber(devicesIds[i]);

            var player = Instantiate(_prefabPlayer).GetComponent<CharController>();
            player.playerId = (CharID)playerNumber;
            _characters[playerNumber] = player;
        }
    }

    void OnMessage(int device_id, JToken data)
    {
        int playerNumber = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

        if (playerNumber == -1)
            return;

        var player = _characters[playerNumber];

        if (player == null || data == null)
            return;

        if (data["horizontal"] != null)
        {
            player.Horizontal = (float)data["horizontal"];
        }

        if (data["bPressed"] != null)
        {
            player.TacklePressed = (bool)data["bPressed"];
        }

        if (data["aPressed"] != null)
        {
            player.JumpPressed = (bool)data["aPressed"];
        }
    }

    void OnDestroy()
    {
        // unregister airconsole events on scene change
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
#endif
    #endregion
}
