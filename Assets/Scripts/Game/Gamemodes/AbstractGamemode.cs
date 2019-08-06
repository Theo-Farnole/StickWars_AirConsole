using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
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
                return new GamemodeDeathMatch(3);
        }

        return null;
    }
}

public abstract class AbstractGamemode
{
    protected int _valueForVictory;
    protected int[] _charactersValue = new int[GameManager.MAX_PLAYERS];
    protected int _indexMvp = -1;

    public AbstractGamemode(int valueForVictory, int initialValue)
    {
        _valueForVictory = valueForVictory;

        for (int i = 0; i < _charactersValue.Length; i++)
        {
            _charactersValue[i] = initialValue;
        }
    }

    public void CheckForVictory()
    {
        for (int i = 0; i < _charactersValue.Length; i++)
        {
            if (_charactersValue[i] >= _valueForVictory)
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
