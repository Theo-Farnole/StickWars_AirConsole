using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelLayoutManager))]
public class LevelLayoutManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawCurrentState();
    }

    private void DrawCurrentState()
    {
        GUILayout.Space(EditorGUIUtility.singleLineHeight);

        var levelLayoutManager = target as LevelLayoutManager;

        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;

        string content = string.Format("Current state is {0}",
            levelLayoutManager.CurrentState != null ? levelLayoutManager.CurrentState.ToString() : "NONE");

        GUILayout.Label(content, centeredStyle);
    }
}
