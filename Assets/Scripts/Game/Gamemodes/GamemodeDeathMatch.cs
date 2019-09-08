using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeDeathMatch : AbstractGamemode
{
    public GamemodeDeathMatch() : base()
    {
        UIManager.Instance.UpdateGamemodeData(_charactersValue);
    }

    public override void Kill(int killerPlayerNumber, int deadPlayerNumber)
    {
        int killerIndex = AirConsole.instance.ConvertDeviceIdToPlayerNumber(killerPlayerNumber);

        if (killerIndex != -1)
        {
            _charactersValue[killerPlayerNumber]++;
        }

        UIManager.Instance.UpdateGamemodeData(_charactersValue);

        CheckForNewMvp(killerPlayerNumber);
        CheckForVictory();

        CameraShake.Instance.Shake();
    }

    protected override void Victory(int winnerPlayerNumber)
    {
        UIManager.Instance.LaunchVictoryAnimation(winnerPlayerNumber);
    }
}
