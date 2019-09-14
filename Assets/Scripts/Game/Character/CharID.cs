using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharID
{
    Red = 0,
    Blue = 1,
    Green = 2,
    Purple = 3
}

public struct CharControls
{
    public KeyCode Left { get; private set; }
    public KeyCode Right { get; private set; }
    public KeyCode Jump { get; private set; }
    public KeyCode Tackle { get; private set; }
    public KeyCode Throw { get; private set; }

    public CharControls(KeyCode left, KeyCode right, KeyCode jump, KeyCode tackle, KeyCode throwProjectile)
    {
        Left = left;
        Right = right;
        Jump = jump;
        Tackle = tackle;
        Throw = throwProjectile;
    }
}

static class CharIDExtensions
{
    public static string GetUIHex(this CharID playerId)
    {
        switch (playerId)
        {
            case CharID.Red:
                return "c40233";

            case CharID.Blue:
                return "0088bf";

            case CharID.Green:
                return "00a568";

            case CharID.Purple:
                return "A60092";
        }

        return "ffffff";
    }

    public static Color GetUIColor(this CharID playerId)
    {
        return playerId.GetUIHex().HexToColor();
    }

    public static string GetSpriteHex(this CharID playerId)
    {
        float goldenRatio = (int)playerId * 0.618033988749895f;

        float r = goldenRatio % 1f;
        float g = 0.5f;
        float b = Mathf.Sqrt(1f - goldenRatio % 0.5f);

        //return Color.HSVToRGB(0.1f * (int)playerId, 0.5f, 1.0f).ToHex();
        return Color.HSVToRGB(r, g, b).ToHex();
    }

    public static Color GetSpriteColor(this CharID playerId)
    {
        return playerId.GetSpriteHex().HexToColor();
    }

    public static CharControls ToControls(this CharID playerId)
    {
        switch (playerId)
        {
            case CharID.Red:
                return new CharControls(KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.Keypad0);

            case CharID.Blue:
                return new CharControls(KeyCode.Q, KeyCode.D, KeyCode.Z, KeyCode.S, KeyCode.E);

            case CharID.Green:
                return new CharControls(KeyCode.K, KeyCode.M, KeyCode.O, KeyCode.L, KeyCode.P);

            case CharID.Purple:
                return new CharControls(KeyCode.Keypad4, KeyCode.Keypad6, KeyCode.Keypad8, KeyCode.Keypad5, KeyCode.Keypad9);
        }

        return new CharControls(KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4);
    }
}