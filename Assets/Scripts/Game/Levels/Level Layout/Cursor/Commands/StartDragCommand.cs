using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDragCommand : AbstractCursorCommand
{
    private readonly Transform _elementToDrag;

    public StartDragCommand(Transform elementToDrag)
    {
        _elementToDrag = elementToDrag;
    }

    public override void Execute(CursorManager cursor)
    {
        cursor.StartDrag(_elementToDrag);
    }

    public override string ToString()
    {
        return string.Format("StartDragCommand => element {0}", _elementToDrag.name);
    }
}
