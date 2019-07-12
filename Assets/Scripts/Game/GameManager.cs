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
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    void OnConnect(int device_id)
    {
        if (AirConsole.instance.GetActivePlayerDeviceIds.Count < 4)
        {
            var activePlayers = AirConsole.instance.GetActivePlayerDeviceIds.Count;

            var player = Instantiate(_prefabPlayer).GetComponent<CharController>();
            player.playerId = (CharID)activePlayers;
            _characters[activePlayers] = player;

            activePlayers++;
            AirConsole.instance.SetActivePlayers(activePlayers);
        }
    }

    void OnMessage(int device_id, JToken data)
    {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

        if (active_player != -1)
        {
            Debug.Log("OnMessge! from " + active_player);
            var player = _characters[active_player];

            if (player == null)
                return;

            if (data["move"] != null)
            {
                player.Horizontal = (float)data["move"];
            }

            if (data["tackle"] != null)
            {
                player.TacklePressed = (bool)data["tackle"];
            }

            if (data["jump"] != null)
            {
                player.JumpPressed = (bool)data["jump"];
            }
        }
    }

    void OnDisconnect(int device_id)
    {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);
        if (active_player != -1)
        {
            if (AirConsole.instance.GetControllerDeviceIds().Count >= 2)
            {
            }
            else
            {
                AirConsole.instance.SetActivePlayers(0);

            }
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
