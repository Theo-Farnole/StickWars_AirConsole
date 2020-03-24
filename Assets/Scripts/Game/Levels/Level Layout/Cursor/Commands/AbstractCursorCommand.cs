using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCursorCommand
{
    public abstract void Execute(CursorManager cursor);
}