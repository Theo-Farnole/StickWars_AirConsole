using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Listen changements on selected gameObject
/// </summary>
[InitializeOnLoad]
class LevelLayoutChangeListener
{

    // TODO: Supporter la selection multiple!

    private static LevelLayoutElement _selectedLevelLayout;
    private static Vector3 _selectedLevelLayout_LastPosition = Vector3.zero;

    static LevelLayoutChangeListener()
    {
        EditorApplication.update += Update;
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void Update()
    {
        if (_selectedLevelLayout == null)
            return;

        bool transformHasChanged = _selectedLevelLayout_LastPosition != _selectedLevelLayout.transform.position;

        if (transformHasChanged)
        {
            _selectedLevelLayout.SaveChanges();
        }
    }

    private static void OnSelectionChanged()
    {
        if (Selection.activeGameObject != null)
        {
            _selectedLevelLayout = Selection.activeGameObject.GetComponent<LevelLayoutElement>();
            
            // set _selectedLevelLayout position, or reset it
            _selectedLevelLayout_LastPosition = _selectedLevelLayout != null ?
                _selectedLevelLayout.transform.position : Vector3.zero;
        }
        else
        {
            _selectedLevelLayout = null;
            _selectedLevelLayout_LastPosition = Vector3.zero;
        }
    }
}