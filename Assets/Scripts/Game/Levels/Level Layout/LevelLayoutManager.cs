using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void LevelLayoutManagerDelegate(LevelLayoutManager levelLayoutManager);

public class LevelLayoutManager : Singleton<LevelLayoutManager>
{
    #region Fields
    [SerializeField] private static int _levelLayoutState = 0;

    [Header("MAIN SETTINGS")]
    [SerializeField] private LevelLayoutData _data;

    [Header("EVENTS")]
    public LevelLayoutManagerDelegate OnLevelLayoutAnimationStart;
    public LevelLayoutManagerDelegate OnLevelLayoutAnimationEnd;

    [Header("COMPONENTS LINKING")]
    [SerializeField] private CursorManager _mainCursor;
    [SerializeField, EnumNamedArray(typeof(CharId))] private CursorManager[] _grabStickmanCursors = new CursorManager[4];

    private bool _isLevelLayoutAnimationRunning = false;
    private bool _disableStartLayout = false;

    private float _timeStartLoadLayout = 0;

    private AbstractState_LevelLayoutManager _currentState;
    #endregion

    #region Properties
    public static int LevelLayoutState
    {
        get => _levelLayoutState;
        set
        {
#if UNITY_EDITOR
            bool levelLayoutStateChanged = _levelLayoutState != value;
#endif

            _levelLayoutState = value;

#if UNITY_EDITOR
            if (levelLayoutStateChanged)
            {
                LoadLayoutWithoutAnimation(value);
            }
#endif
        }
    }

    public AbstractState_LevelLayoutManager CurrentState
    {
        get => _currentState;

        private set
        {
            _currentState?.OnStateExit();

            _currentState = value;

            _currentState?.OnStateEnter();
        }
    }

    public CursorManager[] GrabStickmanCursors { get => _grabStickmanCursors; }
    public CursorManager MainCursor { get => _mainCursor; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        _levelLayoutState = 0;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartLoadLayout();
        }
    }
#endif

    void OnEnable()
    {
        _mainCursor.OnCommandsQueueEmpty += SpreadLevelLayoutAnimationEnded;
        GameManager.Instance.Gamemode.OnCharacterKill += OnCharacterKill;
    }

    void OnDisable()
    {
        _mainCursor.OnCommandsQueueEmpty -= SpreadLevelLayoutAnimationEnded;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Gamemode.OnCharacterKill -= OnCharacterKill;
        }
    }
    #endregion

    #region Load Layout methods
    void StartLoadLayout()
    {
        if (!_data.EnableLevelLayout)
        {
            Debug.LogWarningFormat("You try to start load layout, whereas it's disabled. Tick the property \"Enable Level Layout\" in the database.");
            return;
        }

        _timeStartLoadLayout = Time.time;
        _levelLayoutState++;

        ExtendedAnalytics.SendEvent("Level Layout Triggered", new Dictionary<string, object>()
        {
            { "Time In Session", Time.timeSinceLevelLoad }
        });

        CurrentState = new State_GrabStickmanToNextSpawnPoints(this);

        _isLevelLayoutAnimationRunning = true;
        OnLevelLayoutAnimationStart?.Invoke(this);
    }

#if UNITY_EDITOR
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
            levelData.GizmosEnabled = isCurrentLevelData;
        }
    }
#endif
    #endregion

    #region Events Handler
    void SpreadLevelLayoutAnimationEnded(CursorManager cursorManager)
    {
        if (_isLevelLayoutAnimationRunning)
        {
            OnLevelLayoutAnimationEnd?.Invoke(this);
        }

        _isLevelLayoutAnimationRunning = false;
    }

    void OnCharacterKill(CharId charId)
    {
        if (_disableStartLayout)        
            return;        

        int currentKillsSum = GameManager.Instance.Gamemode.SumCharactersValue;
        int maxKillsSum = GameManager.Instance.Gamemode.MaxKillsPossibleSum;

        if (currentKillsSum >= maxKillsSum * _data.SumRatioToLoadLayout)
        {
            _disableStartLayout = true;
            StartLoadLayout();
        }
    }
    #endregion

    public void GoToNextState()
    {
        if (CurrentState is State_GrabStickmanToNextSpawnPoints)
        {
            CurrentState = new State_LoadLevelLayout(this);
        }
        else if (CurrentState is State_LoadLevelLayout)
        {
            CurrentState = new State_ReleaseStickman(this);
        }
        else if (CurrentState is State_ReleaseStickman)
        {
            CurrentState = null;

            Debug.LogFormat("<color=yellow>Level Layout</color> # Loading ended. Duration {0}", Time.time - _timeStartLoadLayout);

            _isLevelLayoutAnimationRunning = false;
            OnLevelLayoutAnimationEnd?.Invoke(this);
        }
    }

    #region Getter
    public bool DoStickmanCursorsCommandsEmpty()
    {
        for (int i = 0; i < _grabStickmanCursors.Length; i++)
        {
            var cursor = _grabStickmanCursors[i];

            bool isActiveCursor = IsActiveStickmanCursor(cursor);

            if (isActiveCursor && !cursor.IsCommandsEmpty)
                return false;
        }

        return true;
    }

    public bool IsActiveStickmanCursor(CursorManager cursor)
    {
        if (!_grabStickmanCursors.Contains(cursor))
            return false;

        int index = System.Array.IndexOf(_grabStickmanCursors, cursor);
        return IsActiveStickmanCursor(index);
    }

    public bool IsActiveStickmanCursor(int index)
    {
        if (index < 0 || index >= _grabStickmanCursors.Length)
            return false;

        CharId charId = (CharId)index;

        bool isActiveCursor = GameManager.Instance.Characters[charId] != null;

        return isActiveCursor;
    }
    #endregion
    #endregion
}
