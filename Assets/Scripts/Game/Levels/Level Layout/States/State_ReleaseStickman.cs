using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_ReleaseStickman : AbstractState_LevelLayoutManager
{
    public State_ReleaseStickman(LevelLayoutManager owner) : base(owner)
    {
    }

    #region Methods
    #region AbstractState methods
    public override void OnStateEnter()
    {
        ReleaseStickman();         
        RegisterToCursorsEvent();
    }

    public override void OnStateExit()
    {
        RegisterToCursorsEvent(false);
    }
    #endregion

    #region Events Handler
    void RegisterToCursorsEvent(bool registerToEvent = true)
    {
        for (int i = 0; i < _owner.GrabStickmanCursors.Length; i++)
        {
            // ignore non active cursor
            if (!_owner.IsActiveStickmanCursor(i))
                continue;

            var cursor = _owner.GrabStickmanCursors[i];

            if (registerToEvent)
            {
                cursor.OnCommandsQueueEmpty += OnCommandsQueueEmpty;
            }
            else
            {
                cursor.OnCommandsQueueEmpty -= OnCommandsQueueEmpty;
            }
        }
    }

    void OnCommandsQueueEmpty(CursorManager cursorManager)
    {
        TryToGoNextState();
    }
    #endregion

    void ReleaseStickman()
    {
        // foreach stickman
        for (int i = 0; i < GameManager.Instance.Characters.Count; i++)
        {
            var charId = (CharId)i;
            var character = GameManager.Instance.Characters[charId];
            var cursor = _owner.GrabStickmanCursors[i];

            if (character == null)
                continue;

            // unfreeze character 
            character.Freeze = false;
            character.enabled = true;

            Queue<AbstractCursorCommand> cursorCommands = new Queue<AbstractCursorCommand>();

            // cursor commands
            var stopDrag = new StopDragCommand();
            var moveOutScreen = new MoveToCommand(new Vector3(10, cursor.transform.position.y));

            cursorCommands.Enqueue(stopDrag);
            cursorCommands.Enqueue(moveOutScreen);

            cursor.StartCommandsSequence(cursorCommands);
        }
    }

    #region State switching
    void TryToGoNextState()
    {
        if (_owner.DoStickmanCursorsCommandsEmpty())
        {
            _owner.GoToNextState();
        }
    }
    #endregion
    #endregion

}
