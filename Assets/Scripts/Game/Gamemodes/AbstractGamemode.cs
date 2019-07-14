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
                return new GamemodeDeathMatch(10);
        }

        return null;
    }
}

public abstract class AbstractGamemode
{
    protected dynamic _valueForVictory;
    protected dynamic[] _charactersValue = new dynamic[GameManager.MAX_PLAYERS];

    public AbstractGamemode(dynamic valueForVictory, dynamic initialValue)
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
                var device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId(i);
                Victory(device_id);
            }
        }
    }

    protected abstract void Victory(int victory_device_id);
    public abstract void Kill(int killerPlayerNumber, int deadPlayerNumber);
}
