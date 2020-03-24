using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToCommand : AbstractCursorCommand
{
    private readonly Vector3 _position;
    private readonly Transform _transformToGo;

    private Vector3 FinalPosition { get => _transformToGo != null ? _transformToGo.position : _position; }

    public MoveToCommand(Vector3 position)
    {
        _position = position;
    }

    public MoveToCommand(Transform transformToGo)
    {
        _transformToGo = transformToGo;
    }

    public override void Execute(CursorManager cursor)
    {
        cursor.MoveTo(FinalPosition);
    }

    public override string ToString()
    {
        return string.Format("MoveToCommand: position = {0} from {1}", 
            FinalPosition, _transformToGo != null ? "transform from " + _transformToGo.name : "position passed in ctor");
    }
}
