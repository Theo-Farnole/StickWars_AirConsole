using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharController : MonoBehaviour
{
    void Analytics_TriggerProjectileThrow(bool hasEnoughtAmmo)
    {
        return;

        // We disable this event:
        // We are limited to 100 events per hour and projectile are spammed.
        // Moreover, it's 'll disable sending of events with highest priority

        ExtendedAnalytics.SendEvent("Projectile Throw", new Dictionary<string, object>()
        {
            { "Has Enought Ammo", hasEnoughtAmmo },
            { "Thrower Char ID", charId.ToString() }
        });
    }
}
