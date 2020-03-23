using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathzone : MonoBehaviour
{
    private const string LAYER_NAME_IGNORECOLLISION = "Ignore Collision";

    void Start()
    {
#if UNITY_EDITOR
        CheckIfColliderIsSettedCorrectly();
#endif
        ForceLayer();
    }

    void OnValidate()
    {
        ForceLayer();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Entity hitEntity = collision.GetComponent<Entity>();

        if (hitEntity)
        {
            hitEntity.Kill(null);
        }
    }

    #region Debugging
    void CheckIfColliderIsSettedCorrectly()
    {
        var collider = GetComponent<Collider2D>();

        if (collider == null)
        {
            Debug.LogErrorFormat("Deathzone \"{0}\" need a trigger to work properly.");
        }
        else
        {
            if (collider.isTrigger == false)
            {
                Debug.LogErrorFormat("Deathzone \"{0}\"'s collider need to be set as trigger to work properly.");
            }
        }
    }

    private void ForceLayer()
    {
        if (LayerMask.LayerToName(gameObject.layer) != LAYER_NAME_IGNORECOLLISION)
        {
            Debug.LogFormat("{0}'s have been set to {1} to avoid collisions errors.", name, LAYER_NAME_IGNORECOLLISION);
            gameObject.layer = LayerMask.NameToLayer(LAYER_NAME_IGNORECOLLISION);
        }
    }
    #endregion
}
