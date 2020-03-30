using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeDeathMatch : AbstractGamemode
{
    public GamemodeDeathMatch() : base()
    {
    }

    public override void Kill(CharId? killerCharID, CharId victim)
    {
        base.Kill(killerCharID, victim);

        // update score
        if (killerCharID != null)
        {
            CharId c_killerCharID = (CharId)killerCharID;
            _charactersValue[c_killerCharID]++;

            OnScoreUpdate?.Invoke(CharactersValueArray, valueForVictory);
        }

        EventController.Instance.OnKill();
        CameraShake.Instance.Shake();

        CheckForNewMVP(killerCharID);
        CheckForVictory();
    }

    protected override void Victory(CharId winnerCharId)
    {
        UIVictoryManager.Instance.LaunchVictoryAnimation(winnerCharId);
    }
}
