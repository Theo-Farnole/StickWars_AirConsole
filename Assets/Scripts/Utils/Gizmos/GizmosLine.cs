using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code from 
// https://answers.unity.com/questions/1139985/gizmosdrawline-thickens.html
public class GizmosLine : MonoBehaviour
{
    public static void DrawLine(Vector3 p1, Vector3 p2, float width)
    {
        int count = Mathf.CeilToInt(width); // how many lines are needed.
        if (count == 1)
            Gizmos.DrawLine(p1, p2);
        else
        {
            Camera c = Camera.main;

            if (c == null)
            {
                Debug.LogError("Camera.current is null");

                return;
            }

            Vector3 v1 = (p2 - p1).normalized; // line direction
            Vector3 v2 = (c.transform.position - p1).normalized; // direction to camera
            Vector3 n = Vector3.Cross(v1, v2); // normal vector
            for (uint i = 0; i < count; i++)
            {
                Vector3 o = n * width * ((float)i / (count - 1) - 0.5f);
                Gizmos.DrawLine(p1 + o, p2 + o);
            }
        }
    }
}
