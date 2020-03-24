using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelDataLocator
{
    private static LevelData[] _levelData = new LevelData[0];

    public static void RegisterLevelData(LevelData levelData)
    {
        int index = levelData.ActiveOnLayout;

        ResizeIfNeeded(index);

        if (_levelData[index] != null)
        {
            Debug.LogErrorFormat("On register level data, element at {0} is not empty. Abort {1} registering.", index, levelData.name);
            return;
        }

        _levelData[index] = levelData;
    }

    public static LevelData GetLevelData()
    {
        int index = LevelLayoutManager.LevelLayoutState;
        return _levelData[index];
    }

    static void ResizeIfNeeded(int newSize)
    {
        if (newSize >= _levelData.Length)
        {
            Array.Resize(ref _levelData, newSize + 1);
        }
    }
}
