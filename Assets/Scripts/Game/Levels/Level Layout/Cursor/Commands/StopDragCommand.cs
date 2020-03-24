using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopDragCommand : AbstractCursorCommand
{
    public override void Execute(CursorManager cursor)
    {
        cursor.StopDrag();
    }

    public override string ToString()
    {
        return string.Format("StopDragCommand");
    }
}
