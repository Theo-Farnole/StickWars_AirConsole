using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // cache variable
    private SpriteRenderer _spriteRenderer;
    #endregion

    #region Properties
    public SpriteRenderer SpriteRenderer {
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
        InGameCursorFollowRealCursor();
        UpdateCursorSprite();
    }
    #endregion

    private void InGameCursorFollowRealCursor()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        MoveTo(mousePosition);
    }

    void UpdateCursorSprite()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.forward);

        // If hits something
        if (hit.collider != null)
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

    void MoveTo(Vector3 targetPosition)
    {
        // security: lock cursor position
        targetPosition.z = transform.position.z;

        Vector3 direction = targetPosition - transform.position;
        transform.position += direction * (_speed * Time.deltaTime);
    }
    #endregion
}
