﻿using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public static readonly int MAX_PLAYERS = 4;

    #region Fields
    public CharControllerDelegate OnCharacterSpawn;

    [SerializeField] private AllGamemodesData _allGamemodesData;
    [SerializeField] private GameObject _prefabPlayer;
    [Header("Debug")]
    [SerializeField] private bool _enableHotConnection = false;

    private GamemodeType _gamemodeType = GamemodeType.DeathMatch;
    private AbstractGamemode _gamemode;

    private Dictionary<CharId, CharController> _characters = new Dictionary<CharId, CharController>();
    private int _instantiatedCharactersCount = 0;
    #endregion

    #region Properties
    public AbstractGamemode Gamemode { get => _gamemode; }
    public Dictionary<CharId, CharController> Characters { get => _characters; }
    public int InstantiatedCharactersCount { get => _instantiatedCharactersCount; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _gamemode = _gamemodeType.ToGamemodeClass();        
        _gamemode.ValueForVictory = _allGamemodesData.GetGamemodeValue(_gamemodeType);

        foreach (CharId item in Enum.GetValues(typeof(CharId)))
        {
            _characters[item] = null;
        }
#if !UNITY_EDITOR
        _enableHotConnection = false;
#endif
    }

    void Start()
    {        
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
            player.transform.position = LevelLayoutManager.Instance.GetLevelData().GetDefaultSpawnPoint(c_charId);
            player.charId = c_charId;

            _characters[c_charId] = player;
            _instantiatedCharactersCount++;

            OnCharacterSpawn?.Invoke(player);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            UIVictoryManager.Instance.LaunchVictoryAnimation(CharId.Red);
        }
    }
#endif

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
                if (_enableHotConnection)
                {
                    InstantiateCharacter(deviceId, (CharId)charId);
                }
                else
                {
                    var token = new
                    {
                        charId,
                        view = ControllerView.W8GameToFinish.ToString(),
                        bgColor = ((CharId)charId).GetUIHex()
                    };

                    AirConsole.instance.Message(deviceId, token);
                }
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

                if (deviceId != -1)
                {
                    InstantiateCharacter(deviceId, charId);
                }
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
        player.transform.position = LevelLayoutManager.Instance.GetLevelData().GetDefaultSpawnPoint(charId);
        player.ownerDeviceId = deviceId;
        player.charId = charId;

        _characters[charId] = player;
        _instantiatedCharactersCount++;

        // update view
        var token = new
        {
            charId,
            view = ControllerView.Play.ToString(),
            bgColor = charId.GetUIHex()
        };

        AirConsole.instance.Message(deviceId, token);

        Debug.Log("Character instantiated after find free charID: " + charId);
        OnCharacterSpawn?.Invoke(player);
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
                _characters[item].GetComponent<CharacterEntity>().isInvincible = true;
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
