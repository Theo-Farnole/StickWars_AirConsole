using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWindowCommand : AbstractCursorCommand
{
    private const float SECURITY_THRESHOLD = 0.1f;

    private readonly Shortcut _shortcut;

    public OpenWindowCommand(Shortcut shortcut)
    {
        _shortcut = shortcut;
    }

    public override void Execute(CursorManager cursor)
    {
        _shortcut.OpenWindow();

        // wait the shortcut to be opened 
        cursor.ExecuteAfterTime(Shortcut.TRANSITION_DURATION + SECURITY_THRESHOLD, () =>
        {
            cursor.ExecuteNextCommand();
        });
    }
}
