using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayoutElement : MonoBehaviour
{
    [SerializeField] private bool _destroyOnSpecificLayout = false;
    [DrawIf(nameof(_destroyOnSpecificLayout), true, ComparisonType.Equals, DisablingType.ReadOnly)]
    [SerializeField] private int _destroyOnLayoutIndex = 0;

    private Vector3[] _positions = new Vector3[0];

#if UNITY_EDITOR
    public void SaveChanges()
    {
        int layoutState = LevelLayoutManager.LevelLayoutState;

        // prevent Out of bounds exception
        if (layoutState >= _positions.Length)
            Array.Resize(ref _positions, layoutState + 1);

        _positions[layoutState] = transform.position;
    }

    public void LoadLayout()
    {
        int layoutState = LevelLayoutManager.LevelLayoutState;

        // prevent Out of bounds exception
        if (layoutState >= _positions.Length)
            Array.Resize(ref _positions, layoutState + 1);

        bool destroyOnThisLayout = _destroyOnSpecificLayout && layoutState >= _destroyOnLayoutIndex;

        // we should disable the game object,
        // however, FindObjectsOfType doesn't work on disable gameObject

        // so, we just set far the level layout
        Vector3 farPosition = Vector3.right * 20f;
        Vector3 newPosition = destroyOnThisLayout ? farPosition : _positions[layoutState];

        transform.position = newPosition;

        Debug.LogFormat("{0} => destroyOnThisLayout = {1}", name, destroyOnThisLayout);
    }
#endif
}
