using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerID
{
    red = 0,
    blue = 1,
    green = 2,
    yellow = 3
}

static class PlayerIDeExtensions
{
    public static Color ToColor(this PlayerID playerId)
    {
        switch (playerId)
        {
            case PlayerID.red:
                return "c40233".HexToColor();

            case PlayerID.blue:
                return "0088bf".HexToColor();

            case PlayerID.green:
                return "00a568".HexToColor();

            case PlayerID.yellow:
                return "ffd400".HexToColor();
        }

        return Color.white;
    }
}