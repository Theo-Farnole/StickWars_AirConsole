using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDragCommand : AbstractCursorCommand
{
    private readonly Transform _elementToDrag;
    private readonly Action _actionOnDrag;

    public StartDragCommand(Transform elementToDrag, Action actionOnDrag = null)
    {
        _elementToDrag = elementToDrag;
        _actionOnDrag = actionOnDrag;
    }

    public override void Execute(CursorManager cursor)
    {
        _actionOnDrag?.Invoke();
        cursor.StartDrag(_elementToDrag);
    }

    public override string ToString()
    {
        return string.Format("StartDragCommand => element {0}", _elementToDrag.name);
    }
}
