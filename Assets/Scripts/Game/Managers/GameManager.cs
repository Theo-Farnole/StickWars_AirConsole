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

    private GamemodeType gamemodeType = GamemodeType.DeathMatch;
    private AbstractGamemode _gamemode;

    private Dictionary<CharId, CharController> _characters = new Dictionary<CharId, CharController>();

    #endregion

    #region Properties
    public AbstractGamemode Gamemode { get => _gamemode; }
    public Dictionary<CharId, CharController> Characters { get => _characters; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        foreach (CharId item in Enum.GetValues(typeof(CharId)))
        {
            _characters[item] = null;
        }

        // AirConsole callbacks
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

#if UNITY_EDITOR
    void Update()
    {
        CharId? charId = null;

        if (Input.GetKeyDown(KeyCode.Alpha1)) charId = CharId.Red;
        if (Input.GetKeyDown(KeyCode.Alpha2)) charId = CharId.Blue;
        if (Input.GetKeyDown(KeyCode.Alpha3)) charId = CharId.Green;
        if (Input.GetKeyDown(KeyCode.Alpha4)) charId = CharId.Purple;

        if (charId != null && _characters[(CharId)charId] == null)
        {
            CharId c_charId = (CharId)charId;

            var player = Instantiate(_prefabPlayer).GetComponent<CharController>();
            player.transform.position = LevelData.Instance.GetRandomSpawnPoint().position;
            player.charID = c_charId;

            _characters[c_charId] = player;


            UIManager.Instance.SetAvatars();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            UIManager.Instance.LaunchVictoryAnimation(CharId.Red);
        }
    }
#endif

    void OnDestroy()
    {
        AirConsole.instance.onConnect -= OnConnect;
        AirConsole.instance.onReady -= OnReady;
    }
    #endregion

    #region AirConsole events
    void OnConnect(int deviceId)
    {
        CharId? charId = null;

        if (CharIdAllocator.DoDeviceIdExist(deviceId))
        {
            charId = CharIdAllocator.GetCharId(deviceId);
            InstantiateCharacter(deviceId, (CharId)charId);
        }
        else
        {
            charId = CharIdAllocator.AllocateDeviceId(deviceId);

            // Is there available charId
            if (charId != null)
            {
#if UNITY_EDITOR
                InstantiateCharacter(deviceId, (CharId)charId);
#else
            var token = new
            {
                charId,
                view = ControllerView.Wait.ToString(),
                bgColor = ((CharId)charId).GetUIHex()
            };

            AirConsole.instance.Message(deviceId, token);
#endif
            }
            else
            {
                var token = new
                {
                    view = ControllerView.NoPlace.ToString(),
                };

                AirConsole.instance.Message(deviceId, token);
            }
        }
    }

    void OnReady(string str)
    {
        foreach (CharId charId in Enum.GetValues(typeof(CharId)))
        {
            if (CharIdAllocator.DeviceIdToCharId.ContainsKey(charId))
            {
                int deviceId = CharIdAllocator.DeviceIdToCharId[charId];
                InstantiateCharacter(deviceId, charId);
            }
        }
    }
    #endregion

    void InstantiateCharacter(int deviceId, CharId charId)
    {
        if (_characters.ContainsKey(charId) && _characters[charId] != null)
        {
            Debug.LogWarning("Trying to instantiate character already instantiated w/ charId: " + charId);
            return;
        }

        var player = Instantiate(_prefabPlayer).GetComponent<CharController>();
        player.transform.position = LevelData.Instance.GetRandomSpawnPoint().position;
        player.ownerDeviceId = deviceId;
        player.charID = charId;

        _characters[charId] = player;

        // update view
        var token = new
        {
            charId,
            view = ControllerView.Play.ToString(),
            bgColor = charId.GetUIHex()
        };

        AirConsole.instance.Message(deviceId, token);

        Debug.Log("Character instantiated after find free charID: " + charId);
        UIManager.Instance.SetAvatars();
    }

    public void Victory(CharId winnerId)
    {
        // mute & freeze characters
        foreach (CharId item in Enum.GetValues(typeof(CharId)))
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
