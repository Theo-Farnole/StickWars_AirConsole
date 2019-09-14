using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeDeathMatch : AbstractGamemode
{
    public GamemodeDeathMatch() : base()
    {
        UIManager.Instance.UpdateGamemodeData(CharactersValueArray);
    }

    public override void Kill(CharID? killerCharID)
    {
        if (killerCharID == null) return;

        CharID c_killerCharID = (CharID)killerCharID;

        _charactersValue[c_killerCharID]++;

        EventController.Instance.OnKill();

        UIManager.Instance.UpdateGamemodeData(CharactersValueArray);
        CameraShake.Instance.Shake();

        CheckForNewMvp(killerCharID);
        CheckForVictory();
    }

    protected override void Victory(CharID winnerCharId)
    {
        UIManager.Instance.LaunchVictoryAnimation(winnerCharId);
    }
}
