﻿using NDream.AirConsole;
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
        UIManager.Instance.LaunchVictoryAnimation(winnerCharId);
    }
}
