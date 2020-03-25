using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_GrabStickmanToNextSpawnPoints : AbstractState_LevelLayoutManager
{
    private bool _goingToNextState = false;

    public State_GrabStickmanToNextSpawnPoints(LevelLayoutManager owner) : base(owner)
    {
    }

    #region Methods
    #region AbstractState methods
    public override void OnStateEnter()
    {
        RegisterToCursorsEvent();
        GrabStickmanToNextSpawnPoints();
    }

    public override void OnStateExit()
    {        
        RegisterToCursorsEvent(false);
    }
    #endregion

    void GrabStickmanToNextSpawnPoints()
    {
        // foreach stickman
        for (int i = 0; i < GameManager.Instance.Characters.Count; i++)
        {
            var charId = (CharId)i;
            var character = GameManager.Instance.Characters[charId];
            var cursor = _owner.GrabStickmanCursors[i];

            if (character == null)
                continue;

            // freeze character input
            character.Freeze = true;
            character.enabled = false;
            

            Queue<AbstractCursorCommand> cursorCommands = new Queue<AbstractCursorCommand>();

            // cursor commands
            Vector3 spawnPosition = LevelDataLocator.GetLevelData().GetDefaultSpawnPoint(charId);
            void playJumpAnim() => character.Animator.SetBool(CharController.HASH_ANIMATOR_JUMP, true);

            var moveToStickman = new MoveToCommand(character.transform);
            var drag = new StartDragCommand(character.transform, playJumpAnim);
            var moveToSpawnPoint = new MoveToCommand(spawnPosition);

            cursorCommands.Enqueue(moveToStickman);
            cursorCommands.Enqueue(drag);
            cursorCommands.Enqueue(moveToSpawnPoint);

            cursor.StartCommandsSequence(cursorCommands);
        }
    }

    #region Events Handlers
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

    #region State switching
    void TryToGoNextState()
    {
        if (_goingToNextState)
            return;

        if (_owner.DoStickmanCursorsCommandsEmpty())
        {
            _goingToNextState = true; // avoid GoToNextState called 2times
            _owner.GoToNextState();
        }
    }
    #endregion
    #endregion
}
