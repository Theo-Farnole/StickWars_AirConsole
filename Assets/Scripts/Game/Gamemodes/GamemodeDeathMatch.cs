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

        // at the end, because it trigger an event
        base.Kill(killerCharID, victim);
    }

    protected override void Victory(CharId winnerCharId)
    {
        UIVictoryManager.Instance.LaunchVictoryAnimation(winnerCharId);
    }
}
