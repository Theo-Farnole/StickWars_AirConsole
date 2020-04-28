using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent()]
public class LevelLayoutElement : MonoBehaviour
{
    [SerializeField] private int _priorityOrder = 0;
    [SerializeField] private bool _destroyOnSpecificLayout = false;
    [DrawIf(nameof(_destroyOnSpecificLayout), true, ComparisonType.Equals, DisablingType.ReadOnly)]
    [SerializeField] private int _destroyOnLayoutIndex = 0;

    [SerializeField, HideInInspector] private Vector3[] _positions = new Vector3[0];

    public int PriorityOrder { get => _priorityOrder; }

    public Vector3 GetPosition(int layoutState)
    {
        Vector3 farPosition = Vector3.right * 20f;
        bool destroyOnThisLayout = _destroyOnSpecificLayout && layoutState >= _destroyOnLayoutIndex;

        return destroyOnThisLayout ? farPosition : _positions[layoutState];
    }

#if UNITY_EDITOR
    public void SaveChanges()
    {
        Debug.LogFormat("Save changes of {0}", name);

        // prevent Out of bounds exception
        FitPositionArrayInsideLayoutState();

        int layoutState = LevelLayoutManager.Instance.LevelLayoutState;
        _positions[layoutState] = transform.position;
    }

    public void LoadLayout()
    {
        // prevent Out of bounds exception
        FitPositionArrayInsideLayoutState();

        int layoutState = LevelLayoutManager.Instance.LevelLayoutState;

        // we should disable the game object,
        // however, FindObjectsOfType doesn't work on disable gameObject
        // so, we just set far the level layout
        transform.position = GetPosition(layoutState);
    }

    public void FitPositionArrayInsideLayoutState()
    {
        int layoutState = LevelLayoutManager.Instance.LevelLayoutState;
        int oldPositionsLength = _positions.Length;

        // we don't need to resize the array
        if (layoutState < oldPositionsLength)
            return;

        Array.Resize(ref _positions, layoutState + 1);

        // initialize new elements by the current position of the array
        for (int i = oldPositionsLength; i < _positions.Length; i++)
        {
            Debug.LogFormat("populate array at index {0}", i);
            _positions[i] = transform.position;
        }
    }
#endif
}
