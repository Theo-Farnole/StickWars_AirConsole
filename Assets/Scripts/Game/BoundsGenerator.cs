using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsGenerator : MonoBehaviour
{

    void Start()
    {
        // on récupére les positions minimal et maximal en X et Y de la camera
        var camMin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        var camMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        float widthIG = camMax.x - camMin.x;
        float heightIG = camMax.y - camMin.y;

        // on créer un objet qui va avoir les 4 boites de collisions
        GameObject bounds = new GameObject();
        bounds.transform.parent = transform;
        bounds.name = "Bounds";

        // ===
        // On créer les colliders sur les COTES
        Vector2 sideSize = new Vector2(1, heightIG + 5);
        Vector2 sideOffset = new Vector2(widthIG / 2 + 0.5f, 0);

        var left = bounds.AddComponent<BoxCollider2D>();
        left.size = sideSize;
        left.offset = sideOffset;

        var right = bounds.AddComponent<BoxCollider2D>();
        right.size = sideSize;
        right.offset = -sideOffset;


        // ===
        // On créer les colliders sur le HAUT et le BAS
        Vector2 size = new Vector2(widthIG + 5, 1);
        Vector2 offset = new Vector2(0, heightIG / 2 + 0.5f);

        var top = bounds.AddComponent<BoxCollider2D>();
        top.size = size;
        top.offset = offset;

        var bot = bounds.AddComponent<BoxCollider2D>();
        bot.size = size;
        bot.offset = -offset;
    }
}
