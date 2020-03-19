using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LineData
{
    public Vector3 p1 = Vector3.left;
    public Vector3 p2 = Vector3.right;

    public void DrawGizmos()
    {
        Gizmos.DrawLine(p1, p2);
    }

    public Vector3 GetRandomPointInsideLine()
    {
        Vector3 point = Vector3.zero;

        point.x = Random.Range(p1.x, p2.x);
        point.y = Random.Range(p1.y, p2.y);
        point.z = Random.Range(p1.y, p2.z);

        return point;
    }
}

public static class LineArrayExtension
{
    /// <summary>
    /// Select a random element in array, GetRandomPointInsideLine() and return it
    /// </summary>
    public static Vector3 GetRandomPoint(this LineData[] lineArray)
    {
        int randomIndex = Random.Range(0, lineArray.Length);
        return lineArray[randomIndex].GetRandomPointInsideLine();
    }
}