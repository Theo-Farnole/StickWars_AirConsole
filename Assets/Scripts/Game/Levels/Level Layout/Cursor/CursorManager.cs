using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    // drag variables
    private LevelLayoutElement _draggedElement;
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
        // inputs
        Input_FollowRealCursor();
        Input_Drag();

        // movement
        ManageDragMovement();
    }

    void LateUpdate()
    {
        UpdateCursorSprite();
    }
    #endregion

    #region Movements methods
    void Input_FollowRealCursor()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MoveTo(mousePosition);
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        // security: lock cursor position
        targetPosition.z = transform.position.z;

        float distance = Vector3.Distance(transform.position, targetPosition);
        float moveDuration = distance / _speed;

        transform.DOKill();
        transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutCubic);
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
