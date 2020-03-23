using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent()]
public class LevelLayoutElement : MonoBehaviour
{
    [SerializeField] private bool _destroyOnSpecificLayout = false;
    [DrawIf(nameof(_destroyOnSpecificLayout), true, ComparisonType.Equals, DisablingType.ReadOnly)]
    [SerializeField] private int _destroyOnLayoutIndex = 0;

    private Vector3[] _positions = new Vector3[0];

#if UNITY_EDITOR
    public void SaveChanges()
    {
        Debug.LogFormat("Save changes of {0}", name);

        // prevent Out of bounds exception
        FitPositionArrayInsideLayoutState();

        int layoutState = LevelLayoutManager.LevelLayoutState;
        _positions[layoutState] = transform.position;
    }

    public void LoadLayout()
    {
        // prevent Out of bounds exception
        FitPositionArrayInsideLayoutState();


        int layoutState = LevelLayoutManager.LevelLayoutState;
        bool destroyOnThisLayout = _destroyOnSpecificLayout && layoutState >= _destroyOnLayoutIndex;

        // we should disable the game object,
        // however, FindObjectsOfType doesn't work on disable gameObject

        // so, we just set far the level layout
        Vector3 farPosition = Vector3.right * 20f;
        Vector3 newPosition = destroyOnThisLayout ? farPosition : _positions[layoutState];

        transform.position = newPosition;
    }

    public void FitPositionArrayInsideLayoutState()
    {
        int layoutState = LevelLayoutManager.LevelLayoutState;
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
