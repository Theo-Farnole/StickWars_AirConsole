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

public struct PlayerControls
{
    public KeyCode Left { get; private set; }
    public KeyCode Right { get; private set; }
    public KeyCode Jump { get; private set; }
    public KeyCode Tackle { get; private set; }

    public PlayerControls(KeyCode left, KeyCode right, KeyCode jump, KeyCode tackle)
    {
        Left = left;
        Right = right;
        Jump = jump;
        Tackle = tackle;
    }
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

    public static PlayerControls ToControls(this PlayerID playerId)
    {
        switch (playerId)
        {
            case PlayerID.red:
                return new PlayerControls(KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow);

            case PlayerID.blue:
                return new PlayerControls(KeyCode.Q, KeyCode.D, KeyCode.Z, KeyCode.S);

            case PlayerID.green:
                return new PlayerControls(KeyCode.K, KeyCode.M, KeyCode.O, KeyCode.L);

            case PlayerID.yellow:
                return new PlayerControls(KeyCode.Keypad4, KeyCode.Keypad6, KeyCode.Keypad8, KeyCode.Keypad5);
        }

        return new PlayerControls(KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3);
    }
}