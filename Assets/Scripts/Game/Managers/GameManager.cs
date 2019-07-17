using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public static readonly int MAX_PLAYERS = 4;    

    #region Fields
    [SerializeField] private GameObject _prefabPlayer;

    private GamemodeType gamemodeType = GamemodeType.DeathMatch;
    private AbstractGamemode _gamemode;

    private CharController[] _characters = new CharController[MAX_PLAYERS];
    private Dictionary<CharController, int> _charControllerToDeviceID = new Dictionary<CharController, int>();
    #endregion

    #region Properties
    public AbstractGamemode Gamemode { get => _gamemode; }
    public CharController[] Characters { get => _characters; }
    #endregion

    #region MonoBehaviour Callbacks
    void Awake()
    {
        AirConsole.instance.onConnect += OnConnect;

        if (AirConsole.instance.IsAirConsoleUnityPluginReady())
        {
            OnReady(string.Empty);
        }
        else
        {
            AirConsole.instance.onReady += OnReady;
        }
    }

    void Start()
    {
        _gamemode = gamemodeType.ToGamemodeClass();
    }

    void OnDestroy()
    {
        if (AirConsole.instance != null)
        {

            AirConsole.instance.onConnect -= OnConnect;
            AirConsole.instance.onReady -= OnReady;
        }
    }
    #endregion

    #region AirConsole events
    void OnConnect(int device_id)
    {
        InstantiateCharacter(device_id);
    }

    void OnReady(string str)
    {
        var devicesIds = AirConsole.instance.GetControllerDeviceIds();

        for (int i = 0; i < devicesIds.Count; i++)
        {
            InstantiateCharacter(devicesIds[i]);
        }
    }
    #endregion

    void InstantiateCharacter(int device_id)
    {
        // set every player to active player
        var devicesIds = AirConsole.instance.GetControllerDeviceIds();
        AirConsole.instance.SetActivePlayers(devicesIds.Count);

        // convert device id to player numer
        int playerNumber = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);

        // instantiate character is playerNumber is contains inside _characters 
        // and if character hasn't be instantiated
        if (playerNumber < MAX_PLAYERS && _characters[playerNumber] == null)
        {
            var player = Instantiate(_prefabPlayer).GetComponent<CharController>();

            player.playerId = (CharID)playerNumber;
            _characters[playerNumber] = player;

            if (LevelData.Instance == null)
            {
                Debug.LogError("No LevelData on Scene!");
            }
            player.transform.position = LevelData.Instance.GetRandomSpawnPoint().position;
        }

        UIManager.Instance.SetAvatars();
    }
}
