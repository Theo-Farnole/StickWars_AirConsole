using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public static readonly int MAX_PLAYERS = 4;

    #region Fields
    [SerializeField] private GameObject _prefabPlayer;
    [Header("Feedback prefab")]
    [SerializeField] private GameObject _prefabFloatingText;

    private GamemodeType gamemodeType = GamemodeType.DeathMatch;
    private AbstractGamemode _gamemode;

    private Dictionary<CharID, CharController> _characters = new Dictionary<CharID, CharController>();
    private Dictionary<CharID, int> _charControllerToDeviceID = new Dictionary<CharID, int>();
    #endregion

    #region Properties
    public AbstractGamemode Gamemode { get => _gamemode; }
    public GameObject PrefabFloatingText { get => _prefabFloatingText; }
    public Dictionary<CharID, CharController> Characters { get => _characters; }
    public Dictionary<CharID, int> CharControllerToDeviceID { get => _charControllerToDeviceID; set => _charControllerToDeviceID = value; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
#if UNITY_EDITOR
        // add instantiate characters to _characters
        var charControllers = FindObjectsOfType<CharController>();

        for (int i = 0; i < charControllers.Length; i++)
        {
            _characters[charControllers[i].charID] = charControllers[i];
        }

        AirConsole.instance.onConnect += OnConnect;
#endif

        if (AirConsole.instance.IsAirConsoleUnityPluginReady())
        {
            OnReady(string.Empty);
        }
        else
        {
            AirConsole.instance.onReady += OnReady;
        }

        foreach (CharID item in Enum.GetValues(typeof(CharID)))
        {
            _characters[item] = null;
            _charControllerToDeviceID[item] = -1;
        }
    }

    void Start()
    {
        _gamemode = gamemodeType.ToGamemodeClass();
    }

#if UNITY_EDITOR
    void Update()
    {
        CharID? charId = null;

        if (Input.GetKeyDown(KeyCode.Alpha1)) charId = CharID.Red;
        if (Input.GetKeyDown(KeyCode.Alpha2)) charId = CharID.Blue;
        if (Input.GetKeyDown(KeyCode.Alpha3)) charId = CharID.Green;
        if (Input.GetKeyDown(KeyCode.Alpha4)) charId = CharID.Purple;

        if (charId != null && _characters[(CharID)charId] == null)
        {
            CharID c_charId = (CharID)charId;

            var player = Instantiate(_prefabPlayer).GetComponent<CharController>();
            player.transform.position = LevelData.Instance.GetRandomSpawnPoint().position;
            player.charID = c_charId;

            _characters[c_charId] = player;


            UIManager.Instance.SetAvatars();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            UIManager.Instance.LaunchVictoryAnimation(CharID.Red);
        }
    }
#endif

    void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
#if UNITY_EDITOR
            AirConsole.instance.onConnect -= OnConnect;
#endif
            AirConsole.instance.onReady -= OnReady;
        }
    }
    #endregion

    #region AirConsole events
#if UNITY_EDITOR
    void OnConnect(int device_id)
    {
        var activePlayers = AirConsole.instance.GetActivePlayerDeviceIds.Count;

        if (activePlayers < MAX_PLAYERS)
        {
            AirConsole.instance.SetActivePlayers(activePlayers + 1);
            InstantiateCharacter(device_id);

            // update controller
            var token = new
            {
                view = ControllerView.Play.ToString(),
                charID = ((CharID)AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id)).ToString(),
                bgColor = ((CharID)AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id)).GetUIHex()
            };

            AirConsole.instance.Message(device_id, token);
        }
    }
#endif

    void OnReady(string str)
    {
        var devicesIds = AirConsole.instance.GetActivePlayerDeviceIds;

        for (int i = 0; i < devicesIds.Count; i++)
        {
            InstantiateCharacter(devicesIds[i]);
        }
    }
    #endregion

    void InstantiateCharacter(int deviceId)
    {
        bool isActivePlayer = (AirConsole.instance.ConvertDeviceIdToPlayerNumber(deviceId) != -1);

        if (isActivePlayer)
        {
            bool charIdFinded = false;

            // find an availableCharId
            foreach (CharID item in Enum.GetValues(typeof(CharID)))
            {
                if (_characters.ContainsKey(item) && _characters[item] != null)
                    continue;

                charIdFinded = true;

                var player = Instantiate(_prefabPlayer).GetComponent<CharController>();
                player.transform.position = LevelData.Instance.GetRandomSpawnPoint().position;
                player.charID = item;

                _characters[item] = player;
                _charControllerToDeviceID[item] = deviceId;

                Debug.Log("Character instantiated after find free charID: " + item);

                break;
            }

            if (!charIdFinded)
            {
                // on retourne la view "Pas assez de place déso"
            }
        }

        UIManager.Instance.SetAvatars();
    }

    public void Victory(CharID winnerId)
    {
        // mute & freeze characters
        foreach (CharID item in Enum.GetValues(typeof(CharID)))
        {
            if (_characters[item] != null)
            {
                _characters[item].CharAudio.EnableSound = false;
                _characters[item].Freeze = true;
            }
        }

        // destroy every virus
        var virus = FindObjectsOfType<VirusController>();

        for (int i = virus.Length - 1; i >= 0; i--)
        {
            Destroy(virus[i].gameObject);
        }
    }
    #endregion
}
