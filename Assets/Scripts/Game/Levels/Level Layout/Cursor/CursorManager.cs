using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public delegate void CursorManagerDelegate(CursorManager cursorManager);

[RequireComponent(typeof(SpriteRenderer))]
public class CursorManager : MonoBehaviour
{
    #region Enums
    public enum CursorState
    {
        Default,
        Hover,
        Drag
    }
    #endregion

    #region Fields
    public CursorManagerDelegate OnCommandsQueueEmpty;

    public UnityEvent OnDragStart;

    [SerializeField] private float _speed = 3;
    [SerializeField, EnumNamedArray(typeof(CursorState))] private Sprite[] _spritesCursor = new Sprite[3];
    [Header("Feedbacks")]    

    private Queue<AbstractCursorCommand> _commands;

    // drag variables
    private Transform _draggedElement;
    private Vector3 _deltaPositionDraggedElement;
    private Vector3 _lastFramePosition;

    // cache variable
    private SpriteRenderer _spriteRenderer;
    #endregion

    #region Properties
    public RaycastHit2D HitBehindCursor { get => Physics2D.Raycast(transform.position, Vector3.forward); }

    public SpriteRenderer SpriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            return _spriteRenderer;
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Update()
    {
        // movement
        ManageDragMovement();
    }

    void LateUpdate()
    {
        UpdateCursorSprite();
    }
    #endregion

    #region Movements methods
    public void MoveTo(Vector3 targetPosition)
    {
        // security: lock cursor position
        targetPosition.z = transform.position.z;

        float distance = Vector3.Distance(transform.position, targetPosition);
        float moveDuration = distance / _speed;

        transform.DOKill();
        transform.DOMove(targetPosition, moveDuration)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() => ExecuteNextCommand());
    }
    #endregion

    #region Aspects methods
    void UpdateCursorSprite()
    {
        if (_draggedElement != null)
        {
            SetSprite(CursorState.Drag);
        }
        else
        {
            // If hits something
            if (HitBehindCursor.collider != null)
            {
                SetSprite(CursorState.Hover);
            }
            else
            {
                SetSprite(CursorState.Default);
            }
        }
    }

    void SetSprite(CursorState cursorState)
    {
        int index = (int)cursorState;
        Sprite sprite = _spritesCursor[index];

        SpriteRenderer.sprite = sprite;
    }
    #endregion

    #region Dragging methods
    void ManageDragMovement()
    {
        if (_draggedElement != null)
        {
            Vector3 delta = transform.position - _lastFramePosition;
            DragGameObject(delta);
        }

        _lastFramePosition = transform.position;
    }

    void DragGameObject(Vector3 offset)
    {
        if (_draggedElement == null)
            return;

        _draggedElement.transform.position += offset;
    }

    public void StartDrag(Transform element)
    {
        if (element == null)
        {
            Debug.LogWarningFormat("{0} start drag of null element! Please use StopDrag() if you want to stop dragging.", name);
            return;
        }

        _draggedElement = element;
        _deltaPositionDraggedElement = transform.position - element.transform.position;

        OnDragStart?.Invoke();

        ExecuteNextCommand();
    }

    public void StopDrag()
    {
        _draggedElement = null;

        ExecuteNextCommand();
    }
    #endregion

    #region Commands methods
    public void ExecuteNextCommand()
    {
        if (_commands == null || _commands.Count == 0)
        {
            Debug.LogFormat("{0} OnCommandsQueueEmpty", name);
            OnCommandsQueueEmpty?.Invoke(this);
            return;
        }

        var nextCommand = _commands.Dequeue();
        nextCommand.Execute(this);

        //Debug.LogFormat("ExecuteNextCommand passed w/ command of type {0} => {1}.",
        //    nextCommand.GetType(), nextCommand.ToString());

        //Debug.Break();
    }

    public void StartCommandsSequence(Queue<AbstractCursorCommand> cursorCommands)
    {
        Debug.LogFormat("{0} starts a sequence of {1} commands.", name, cursorCommands.Count);

        _commands = cursorCommands;
        ExecuteNextCommand();
    }
    #endregion

    #region Getter
    public bool IsCommandsEmpty => _commands.Count == 0;
    #endregion
    #endregion
}
