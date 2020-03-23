using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrownPositionManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private Entity _entity;
    [SerializeField] private Vector3 _copyPosition;

    private bool _isAtCopyPosition = false;
    private Vector3 _originalLocalPosition = Vector3.zero;

    // cache variable
    FancyObject _fancyObject;
    #endregion

    #region Properties
    private bool IsAtOriginalPosition { get => !_isAtCopyPosition; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _fancyObject = GetComponent<FancyObject>();
    }

    void Start()
    {
        _originalLocalPosition = transform.localPosition;
        GotoSliderPosition();
    }

    void OnEnable()
    {
        _entity.OnHealthPointsChanged += OnHealthPointsChanged;
    }

    void OnDisable()
    {
        _entity.OnHealthPointsChanged -= OnHealthPointsChanged;
    }

    void OnDrawGizmosSelected()
    {
        const float sphereRadius = 0.1f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_copyPosition, sphereRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.localPosition, sphereRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.localPosition, _copyPosition);
    }
    #endregion

    #region Events Handlers
    void OnHealthPointsChanged(Entity entity)
    {
        if (entity.HealthSlider.gameObject.activeInHierarchy)
        {
            ReturnToOriginalPosition();
        }
        else
        {
            GotoSliderPosition();
        }
    }
    #endregion

    void ReturnToOriginalPosition()
    {
        if (IsAtOriginalPosition)
            return;

        SetNewPosition(_originalLocalPosition);
    }

    void GotoSliderPosition()
    {
        if (_isAtCopyPosition)
            return;

        _isAtCopyPosition = true;

        _originalLocalPosition = transform.localPosition;
        SetNewPosition(_copyPosition);

        Debug.DrawLine(transform.position + _originalLocalPosition, transform.position + _originalLocalPosition + Vector3.right, Color.magenta, Mathf.Infinity);
    }

    void SetNewPosition(Vector3 position)
    {
        transform.localPosition = position;
        _fancyObject?.ResetStartingPosition(position);
    }
    #endregion
}
