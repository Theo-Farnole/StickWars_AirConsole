using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeDeathMatch : AbstractGamemode
{
    public GamemodeDeathMatch() : base()
    {
    }

    public override void Kill(CharId? killerCharID)
    {
        base.Kill(killerCharID);

        if (killerCharID == null) return;

        CharId c_killerCharID = (CharId)killerCharID;

        _charactersValue[c_killerCharID]++;

        EventController.Instance.OnKill();

        CameraShake.Instance.Shake();

        OnScoreUpdate?.Invoke(CharactersValueArray, valueForVictory);

        CheckForNewMVP(killerCharID);
        CheckForVictory();
    }

    protected override void Victory(CharId winnerCharId)
    {
        UIManager.Instance.LaunchVictoryAnimation(winnerCharId);
    }
}
