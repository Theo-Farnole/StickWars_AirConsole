using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State_LoadLevelLayout : AbstractState_LevelLayoutManager
{
    public State_LoadLevelLayout(LevelLayoutManager owner) : base(owner)
    {
    }

    public override void OnStateEnter()
    {
        LoadLayoutWithAnimation();

        _owner.MainCursor.OnCommandsQueueEmpty += OnCommandsQueueEmpty;
    }

    public override void OnStateExit()
    {
        _owner.MainCursor.OnCommandsQueueEmpty -= OnCommandsQueueEmpty;
    }

    void OnCommandsQueueEmpty(CursorManager cursorManager)
    {
        _owner.GoToNextState();
    }

    private void LoadLayoutWithAnimation()
    {
        Queue<AbstractCursorCommand> cursorCommands = new Queue<AbstractCursorCommand>();

        // destroy pickup & errors virus
        DestroyCollectiblesAndBonus();

        // open every shortcut
        OpenShortcuts(ref cursorCommands);

        // then, move layout
        MoveLevelLayout(ref cursorCommands);

        //Debug.LogFormat("Executing {0} commands.", cursorCommands.Count);
        _owner.MainCursor.StartCommandsSequence(cursorCommands);
    }

    #region Layout modifications methods
    void DestroyCollectiblesAndBonus()
    {
        GameHelper.DestroyGameObjectsInScene<ProjectilePickup>();
        GameHelper.DestroyGameObjectsInScene<VirusSpawner>();
        GameHelper.DestroyGameObjectsInScene<VirusController>();
        GameHelper.DestroyGameObjectsInScene<Projectile>();
    }

    void OpenShortcuts(ref Queue<AbstractCursorCommand> cursorCommands)
    {
        // Shortcut must be open to don't forget window
        var shortcuts = GameObject.FindObjectsOfType<Shortcut>();

        for (int i = 0; i < shortcuts.Length; i++)
        {
            var shortcut = shortcuts[i];

            var moveToShortcut = new MoveToCommand(shortcut.transform.position);
            var openWindow = new OpenWindowCommand(shortcut);

            cursorCommands.Enqueue(moveToShortcut);
            cursorCommands.Enqueue(openWindow);
        }
    }

    void MoveLevelLayout(ref Queue<AbstractCursorCommand> cursorCommands)
    {
        var levelLayoutElements = GameObject.FindObjectsOfType<LevelLayoutElement>();

        levelLayoutElements = levelLayoutElements.OrderBy(x => x.PriorityOrder).ToArray();

        for (int i = 0; i < levelLayoutElements.Length; i++)
        {
            var element = levelLayoutElements[i];

            var moveToWindow = new MoveToCommand(element.transform);
            var dragWindow = new StartDragCommand(element.transform);
            var moveToNewPosition = new MoveToCommand(element.GetPosition(LevelLayoutManager.LevelLayoutState));
            var stopDrag = new StopDragCommand(); // OPTIMIZATION: sortir cette line de la loop

            cursorCommands.Enqueue(moveToWindow);
            cursorCommands.Enqueue(dragWindow);
            cursorCommands.Enqueue(moveToNewPosition);
            cursorCommands.Enqueue(stopDrag);
        }

        var moveOutScreen = new MoveToCommand(Vector3.right * 10);
        cursorCommands.Enqueue(moveOutScreen);
    }
    #endregion

}
