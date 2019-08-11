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
    public static int valueForVictory = 3;
    protected int[] _charactersValue = new int[GameManager.MAX_PLAYERS];
    protected int _indexMvp = -1;

    public AbstractGamemode()
    {
        _charactersValue = Enumerable.Repeat(0, GameManager.MAX_PLAYERS).ToArray();
    }

    public void CheckForVictory()
    {
        for (int i = 0; i < _charactersValue.Length; i++)
        {
            if (_charactersValue[i] >= valueForVictory)
            {
                Victory(i);
            }
        }
    }

    protected void CheckForNewMvp(int indexPlayerWithNewScore)
    {
        if (_indexMvp == -1 || _charactersValue[_indexMvp] < _charactersValue[indexPlayerWithNewScore])
        {
            if (_indexMvp != -1)
            {
                GameManager.Instance.Characters[_indexMvp].IsMVP = false;
            }

            _indexMvp = indexPlayerWithNewScore;

            GameManager.Instance.Characters[_indexMvp].IsMVP = true;
        }
    }

    protected abstract void Victory(int winnerPlayerNumber);
    public abstract void Kill(int killerPlayerNumber, int deadPlayerNumber);
}
