using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class StatisticsManager : MonoBehaviour
{
    private Dictionary<CharId, CharacterStatistics> _characterStatistics = new Dictionary<CharId, CharacterStatistics>();

    public Dictionary<CharId, CharacterStatistics> CharacterStatistics { get => _characterStatistics; }



    #region Methods
    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        Assert.IsNotNull(GameManager.Instance);

        GameManager.Instance.OnCharacterSpawn += OnCharacterSpawn;
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCharacterSpawn -= OnCharacterSpawn;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DebugAllStatistics();
        }
    }
#endif
    #endregion

    #region Events handlers
    void OnCharacterSpawn(CharController charController)
    {
        CharId charId = charController.charId;

        Assert.IsFalse(_characterStatistics.ContainsKey(charId), string.Format("CharId {0} is already a key in characterStatistics dictionary.", charId));

        CharacterStatistics characterStatistics = new CharacterStatistics(charId, charController);
        _characterStatistics.Add(charId, characterStatistics);
    }
    #endregion

    #region Debugs
    void DebugAllStatistics()
    {
        Debug.LogFormat("Display statistics of {0} players", _characterStatistics.Count);

        foreach (var kvp in _characterStatistics)
        {
            Debug.Log(kvp.Value.ToString());
        }
    }

    void DebugStatistics(CharId charId)
    {
        if (!_characterStatistics.ContainsKey(charId))
            return;

        _characterStatistics[charId].ToString();
    }
    #endregion
    #endregion
}
