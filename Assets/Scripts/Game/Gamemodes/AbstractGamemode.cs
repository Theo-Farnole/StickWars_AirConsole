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

public abstract class AbstractGamemode
{
    #region Fields
    public static int valueForVictory = 5;
    protected Dictionary<CharId, int> _charactersValue = new Dictionary<CharId, int>();
    protected CharId? _mvpCharID = null;
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
            int[] charactersValueArray = new int [Enum.GetValues(typeof(CharId)).Length];

            foreach (CharId item in Enum.GetValues(typeof(CharId)))
            {
                charactersValueArray[(int)item] = _charactersValue[item];
            }

            return charactersValueArray;
        }
    }
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

    protected void CheckForNewMvp(CharId? playerToCheck)
    {
        if (_mvpCharID == null || playerToCheck == null) return;

        CharId mvpCharID = (CharId)_mvpCharID;
        CharId newMvpToCheck = (CharId)playerToCheck;

        if (_charactersValue[mvpCharID] < _charactersValue[newMvpToCheck])
        {
            GameManager.Instance.Characters[mvpCharID].IsMVP = false;

            _mvpCharID = playerToCheck;

            GameManager.Instance.Characters[mvpCharID].IsMVP = true;
        }
    }

    protected abstract void Victory(CharId winnerID);
    public abstract void Kill(CharId? killerCharID);
    #endregion
}
