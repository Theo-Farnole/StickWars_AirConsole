using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelDataLocator
{
    private static LevelData[] _levelData = null;

    public static LevelData GetLevelData()
    {
        if (_levelData == null)
        {
            InitializeLevelData();
        }

        int index = LevelLayoutManager.LevelLayoutState;
        return _levelData[index];
    }

    static void InitializeLevelData()
    {
        var levelDatas = GameObject.FindObjectsOfType<LevelData>();
        _levelData = levelDatas.OrderBy(x => x.ActiveOnLayout).ToArray();
    }
}
