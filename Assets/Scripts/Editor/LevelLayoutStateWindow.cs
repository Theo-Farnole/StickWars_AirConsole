using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelLayoutStateWindow : EditorWindow
{

    #region ctor
    [MenuItem("StickWars/Open Level Layout State")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LevelLayoutStateWindow window = (LevelLayoutStateWindow)EditorWindow.GetWindow(typeof(LevelLayoutStateWindow));
        window.Show();
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    void OnDisable()
    {
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);

        var inputLevelLayoutState = EditorGUILayout.IntSlider("Slider", LevelLayoutManager.LevelLayoutState, 0, 1);

        if (inputLevelLayoutState != LevelLayoutManager.LevelLayoutState)
        {
            LevelLayoutManager.LevelLayoutState = inputLevelLayoutState;
            EditorPrefs.SetInt("LevelLayoutState", inputLevelLayoutState);
        }
    }
    #endregion

    #region Events handlers
    public void OnAfterAssemblyReload()
    {
        LevelLayoutManager.LevelLayoutState = EditorPrefs.GetInt("LevelLayoutState", 0);
    }
    #endregion
    #endregion
}