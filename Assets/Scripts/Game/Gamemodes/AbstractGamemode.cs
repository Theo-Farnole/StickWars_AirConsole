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
    protected Dictionary<CharID, int> _charactersValue = new Dictionary<CharID, int>();
    protected CharID? _mvpCharID = null;
    #endregion

    #region Properties
    public int SumCharactersValue
    {
        get
        {
            int sum = 0;

            foreach (CharID item in Enum.GetValues(typeof(CharID)))
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
            int[] charactersValueArray = new int [Enum.GetValues(typeof(CharID)).Length];

            foreach (CharID item in Enum.GetValues(typeof(CharID)))
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
        foreach (CharID item in Enum.GetValues(typeof(CharID)))
        {
            _charactersValue[item] = 0;
        }
    }

    public bool CheckForVictory()
    {
        foreach (CharID item in Enum.GetValues(typeof(CharID)))
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

    protected void CheckForNewMvp(CharID? playerToCheck)
    {
        if (_mvpCharID == null || playerToCheck == null) return;

        CharID mvpCharID = (CharID)_mvpCharID;
        CharID newMvpToCheck = (CharID)playerToCheck;

        if (_charactersValue[mvpCharID] < _charactersValue[newMvpToCheck])
        {
            GameManager.Instance.Characters[mvpCharID].IsMVP = false;

            _mvpCharID = playerToCheck;

            GameManager.Instance.Characters[mvpCharID].IsMVP = true;
        }
    }

    protected abstract void Victory(CharID winnerID);
    public abstract void Kill(CharID? killerCharID);
    #endregion
}
