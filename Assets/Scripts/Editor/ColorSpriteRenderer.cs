using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ColorSpriteRenderer
{
    private static CharId charId = CharId.Red;

    [MenuItem("Tools/Color Sprite Renderer")]
    static void ColorSelectedSprite()
    {
        var spriteRenderer = Selection.activeGameObject.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            return;

        spriteRenderer.color = GetNextCharID().GetSpriteColor();             
    }
    
    static CharId GetNextCharID()
    {
        Debug.LogFormat("charId = {0}", charId);

        if (charId == CharId.Purple)
        {
            charId = CharId.Red;
        }
        else
        {
            charId = (CharId)charId + 1;
            Debug.LogFormat("CharId ++; {0}", charId);
        }

        return charId;
    }
}
