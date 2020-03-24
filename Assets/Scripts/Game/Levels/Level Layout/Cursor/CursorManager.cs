using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: update _draggedGameObject position in Update

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
    [SerializeField] private float _speed = 3;
    [SerializeField, EnumNamedArray(typeof(CursorState))] private Sprite[] _spritesCursor = new Sprite[3];

    // move to variable
    private bool _isMoving = false;
    private Vector3 _directionToMoveToPosition;
    private Vector3 _moveToPosition;

    // drag variables
    private LevelLayoutElement _draggedElement;
    private Vector3 _deltaPositionDraggedElement;

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
        // inputs
        Input_FollowRealCursor();
        Input_Drag();

        // movements
        ManageMovement();
    }

    void LateUpdate()
    {
        UpdateCursorSprite();
    }
    #endregion

    #region Movements methods
    void Input_FollowRealCursor()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        MoveTo(mousePosition);
    }

    void ManageMovement()
    {
        if (_isMoving)
        {
            Vector3 delta = _directionToMoveToPosition * (_speed * Time.deltaTime);

            transform.position += delta;
            DragGameObject(delta);
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        // security: lock cursor position
        targetPosition.z = transform.position.z;

        _directionToMoveToPosition = (targetPosition - transform.position).normalized;
        _moveToPosition = targetPosition;

        StartMovement();
    }

    public void StartMovement()
    {
        _isMoving = true;
    }

    public void StopMovement()
    {
        _isMoving = false;
    }
    #endregion

    #region Aspects methods
    void UpdateCursorSprite()
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

    void SetSprite(CursorState cursorState)
    {
        int index = (int)cursorState;
        Sprite sprite = _spritesCursor[index];

        SpriteRenderer.sprite = sprite;
    }
    #endregion

    #region Dragging methods
    void Input_Drag()
    {
        var cachedHit = HitBehindCursor;

        if (Input.GetMouseButtonDown(0) && cachedHit.collider != null)
        {
            StopDrag();

            StartDrag(cachedHit.collider.gameObject.GetComponentInParent<LevelLayoutElement>());
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }
    }

    void DragGameObject(Vector3 offset)
    {
        if (_draggedElement == null)
            return;

        _draggedElement.transform.position += offset;
    }

    public void StartDrag(LevelLayoutElement levelLayoutElement)
    {
        if (levelLayoutElement == null)
        {
            Debug.LogWarningFormat("{0} start drag of null element! Please use StopDrag() if you want to stop dragging.", name);
            return;
        }

        _draggedElement = levelLayoutElement;

        _deltaPositionDraggedElement = transform.position - levelLayoutElement.transform.position;
    }

    public void StopDrag()
    {
        _draggedElement = null;
    }
    #endregion
    #endregion
}
