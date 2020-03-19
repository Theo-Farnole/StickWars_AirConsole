using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        var hitCharController = collision.GetComponent<CharController>();

        if (hitCharController)
        {
            hitCharController.FillCarriedProjectilesAmount();
            Destroy(gameObject);
        }
    }
}
