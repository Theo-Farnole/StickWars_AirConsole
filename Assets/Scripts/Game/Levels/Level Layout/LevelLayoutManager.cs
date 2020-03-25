using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void LevelLayoutManagerDelegate(LevelLayoutManager levelLayoutManager);

public class LevelLayoutManager : Singleton<LevelLayoutManager>
{
    #region Fields
    [SerializeField] private static int _levelLayoutState = 0;

    public LevelLayoutManagerDelegate OnLevelLayoutAnimationStart;
    public LevelLayoutManagerDelegate OnLevelLayoutAnimationEnded;

    [SerializeField] private CursorManager _cursor;
    [Header("DEBUG")]
    [SerializeField] private KeyCode _triggerLevelLayoutLoading = KeyCode.R;

    private bool _isLevelLayoutAnimationRunning = false;
    #endregion

    #region Properties
    public static int LevelLayoutState
    {
        get => _levelLayoutState;
        set
        {
            bool levelLayoutStateChanged = _levelLayoutState != value;

            _levelLayoutState = value;

            if (levelLayoutStateChanged)
            {
                LoadLayoutWithoutAnimation(value);
            }
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        _levelLayoutState = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(_triggerLevelLayoutLoading))
        {
            _levelLayoutState++;
            LoadLayoutWithAnimation();
        }
    }

    void OnEnable()
    {
        _cursor.OnCommandsQueueEmpty += OnCommandsQueueEmpty;
    }

    void OnDisable()
    {
        _cursor.OnCommandsQueueEmpty -= OnCommandsQueueEmpty;
    }
    #endregion

    #region Load layout
    private void LoadLayoutWithAnimation()
    {
        Queue<AbstractCursorCommand> cursorCommands = new Queue<AbstractCursorCommand>();

        // destroy pickup & errors virus
        DestroyCollectiblesAndBonus();

        // open every shortcut
        OpenShortcuts(ref cursorCommands);

        // then, move layout
        MoveLevelLayout(ref cursorCommands);

        Debug.LogFormat("Executing {0} commands.", cursorCommands.Count);
        _cursor.StartCommandsSequence(cursorCommands);

        _isLevelLayoutAnimationRunning = true;
        OnLevelLayoutAnimationStart?.Invoke(this);
    }

    public static void LoadLayoutWithoutAnimation(int layout)
    {
        var levelLayoutElements = GameObject.FindObjectsOfType<LevelLayoutElement>();

        foreach (var element in levelLayoutElements)
        {
            element.LoadLayout();
        }

        var levelDatas = FindObjectsOfType<LevelData>();

        foreach (var levelData in levelDatas)
        {
            bool isCurrentLevelData = levelData.ActiveOnLayout == _levelLayoutState;
            levelData.enabled = isCurrentLevelData;
        }
    }
    #endregion

    #region Events Handler
    void OnCommandsQueueEmpty(CursorManager cursorManager)
    {
        if (_isLevelLayoutAnimationRunning)
        {
            OnLevelLayoutAnimationEnded?.Invoke(this);
        }

        _isLevelLayoutAnimationRunning = false;
    }
    #endregion

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
            var dragWindow = new StartDragCommand(element);
            var moveToNewPosition = new MoveToCommand(element.GetPosition(_levelLayoutState));
            var stopDrag = new StopDragCommand(); // OPTIMIZATION: sortir cette line de la loop

            cursorCommands.Enqueue(moveToWindow);
            cursorCommands.Enqueue(dragWindow);
            cursorCommands.Enqueue(moveToNewPosition);
            cursorCommands.Enqueue(stopDrag);
        }
    }
    #endregion
    #endregion
}
