using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharController : MonoBehaviour
{
    void Analytics_TriggerProjectileThrow(bool hasEnoughtAmmo)
    {
        ExtendedAnalytics.SendEvent("Projectile Throw", new Dictionary<string, object>()
        {
            { "Has Enought Ammo", hasEnoughtAmmo },
            { "Thrower Char ID", charId.ToString() }
        });
    }
}
