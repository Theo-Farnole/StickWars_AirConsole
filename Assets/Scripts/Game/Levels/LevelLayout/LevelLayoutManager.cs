using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayoutManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private static int _levelLayoutState = 0;
    #endregion

    #region Properties
    public static int LevelLayoutState
    {
        get => _levelLayoutState;
        set
        {
            bool levelLayoutStateChanged = _levelLayoutState != value;
            
            _levelLayoutState = value;

            if (levelLayoutStateChanged)
            {
                LoadLayoutWithoutAnimation(value);
            }
        }
    }
    #endregion

    #region Methods
    public static void LoadLayoutWithoutAnimation(int layout)
    {
        //Debug.LogFormat("Loading layout {0}...", layout);

        var levelLayoutElements = GameObject.FindObjectsOfType<LevelLayoutElement>();

        foreach (var element in levelLayoutElements)
        {
            element.LoadLayout();
        }

        //Debug.LogFormat("Loading of layout {0} ended.", layout);
    }
    #endregion
}
