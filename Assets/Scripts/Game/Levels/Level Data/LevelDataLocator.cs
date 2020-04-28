using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelDataLocator
{
    public static LevelData GetLevelData()
    {
        return LevelLayoutManager.Instance.GetLevelData();
    }
}
