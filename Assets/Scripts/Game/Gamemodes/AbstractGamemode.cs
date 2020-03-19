using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GamemodeType
{
    DeathMatch
}

public static class GamemodeTypeExtension
{
    public static AbstractGamemode ToGamemodeClass(this GamemodeType g)
    {
        switch (g)
        {
            case GamemodeType.DeathMatch:
                return new GamemodeDeathMatch();
        }

        return null;
    }
}

public delegate void IntArrayDelegate(int[] score, int scoreForVictory);

public abstract class AbstractGamemode
{
    #region Fields
    public static int valueForVictory = 8;

    public CharIdDelegate OnCharacterKill;
    public IntArrayDelegate OnScoreUpdate;

    protected Dictionary<CharId, int> _charactersValue = new Dictionary<CharId, int>();
    protected CharId? _currentMVPCharID = null;

    private int _killCount = 0;
    #endregion

    #region Properties
    public int SumCharactersValue
    {
        get
        {
            int sum = 0;

            foreach (CharId item in Enum.GetValues(typeof(CharId)))
            {
                sum += _charactersValue[item];
            }

            return sum;
        }
    }

    public int[] CharactersValueArray
    {
        get
        {
            int[] charactersValueArray = new int[Enum.GetValues(typeof(CharId)).Length];

            foreach (CharId item in Enum.GetValues(typeof(CharId)))
            {
                charactersValueArray[(int)item] = _charactersValue[item];
            }

            return charactersValueArray;
        }
    }

    public int ValueForVictory { get => valueForVictory; }
    public int KillCount { get => _killCount; }
    #endregion

    #region Methods
    public AbstractGamemode()
    {
        // init _charactersValue
        foreach (CharId item in Enum.GetValues(typeof(CharId)))
        {
            _charactersValue[item] = 0;
        }
    }

    public int GetPlayersCountAtScore(int score)
    {
        return _charactersValue.Select(x => x.Value == score).Count();
    }

    public int GetPositionInPlayersAtScore(CharId charId)
    {
        int charIdScore = _charactersValue[charId];

        var array = _charactersValue
                        .Where(x => x.Value == charIdScore)
                        .OrderByDescending(x => x.Key)
                        .Select(x => x.Key)
                        .ToArray();

        return Array.FindIndex(array, x => x == charId);
    }

    public bool CheckForVictory()
    {
        foreach (CharId item in Enum.GetValues(typeof(CharId)))
        {
            if (_charactersValue[item] >= valueForVictory)
            {
                GameManager.Instance.Victory(item);
                Victory(item);

                return true;
            }
        }

        return false;
    }

    protected void CheckForNewMVP(CharId? charIDToCheck)
    {
        // prevent check for new mvp is charID is null
        if (charIDToCheck == null) return;

        // if there is no current mvp, let charId become the mvp
        if (_currentMVPCharID == null)
        {
            _currentMVPCharID = charIDToCheck;
            GameManager.Instance.Characters[(CharId)charIDToCheck].IsMVP = true;
            return;
        }

        // converting from CharId? to CharId
        CharId currentMVPCharIDConverted = (CharId)_currentMVPCharID;
        CharId charIDToCheckConverted = (CharId)charIDToCheck;

        if (_charactersValue[currentMVPCharIDConverted] < _charactersValue[charIDToCheckConverted])
        {
            // remove isMvp status from old MVP
            GameManager.Instance.Characters[currentMVPCharIDConverted].IsMVP = false;

            // set new MVP
            _currentMVPCharID = charIDToCheck;
            currentMVPCharIDConverted = charIDToCheckConverted;

            // set isMvp to true to new MVP
            GameManager.Instance.Characters[charIDToCheckConverted].IsMVP = true;
        }
    }

    protected abstract void Victory(CharId winnerID);

    public virtual void Kill(CharId? killerCharID)
    {
        _killCount++;

        if (killerCharID != null)
        {
            OnCharacterKill?.Invoke((CharId)killerCharID);
        }
    }
    #endregion
}
