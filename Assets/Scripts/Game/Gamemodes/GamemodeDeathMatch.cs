using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeDeathMatch : AbstractGamemode
{
    public GamemodeDeathMatch(int killsForVictory) : base(killsForVictory, 0)
    {
        UIManager.Instance.UpdateGamemodeData(_charactersValue);
    }

    public override void Kill(int killerPlayerNumber, int deadPlayerNumber)
    {
        int killerIndex = AirConsole.instance.ConvertDeviceIdToPlayerNumber(killerPlayerNumber);
        _charactersValue[killerPlayerNumber]++;

        UIManager.Instance.UpdateGamemodeData(_charactersValue);

        CheckForVictory();
    }

    protected override void Victory(int winnerDeviceId)
    {
        Debug.Log("Victory!");
    }
}
