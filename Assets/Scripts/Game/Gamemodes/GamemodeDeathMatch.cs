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

    public override void Kill(CharId? killerCharID)
    {
        if (killerCharID == null) return;

        CharId c_killerCharID = (CharId)killerCharID;

        _charactersValue[c_killerCharID]++;

        EventController.Instance.OnKill();

        UIManager.Instance.UpdateGamemodeData(CharactersValueArray);
        CameraShake.Instance.Shake();

        CheckForNewMvp(killerCharID);
        CheckForVictory();
    }

    protected override void Victory(CharId winnerCharId)
    {
        UIManager.Instance.LaunchVictoryAnimation(winnerCharId);
    }
}
