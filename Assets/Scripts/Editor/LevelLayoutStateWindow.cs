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

        if (LevelLayoutManager.Instance == null)
            GUILayout.Label("You must have a LevelLayoutManager in your scene to use Level Layout window.");

        var inputLevelLayoutState = EditorGUILayout.IntSlider("Slider", LevelLayoutManager.Instance.LevelLayoutState, 0, 1);

        if (inputLevelLayoutState != LevelLayoutManager.Instance.LevelLayoutState)
        {
            LevelLayoutManager.Instance.LevelLayoutState = inputLevelLayoutState;
            EditorPrefs.SetInt("LevelLayoutState", inputLevelLayoutState);
        }
    }
    #endregion

    #region Events handlers
    public void OnAfterAssemblyReload()
    {
        if (LevelLayoutManager.Instance != null)
        {
            LevelLayoutManager.Instance.LevelLayoutState = EditorPrefs.GetInt("LevelLayoutState", 0);
        }
    }
    #endregion
    #endregion
}