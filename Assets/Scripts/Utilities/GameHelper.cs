using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameHelper
{
    public static void DestroyGameObjectsInScene<T>() where T : MonoBehaviour
    {
        T[] gameObjects = (T[])GameObject.FindObjectsOfType(typeof(T));

        for (int i = gameObjects.Length - 1; i >= 0; i--)
        {
            GameObject.Destroy(gameObjects[i].gameObject);
        }
    }
}
