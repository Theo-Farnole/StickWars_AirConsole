using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterAttackTypeExtension
{
    public static AttackType ToAttackType(CharacterAttackType attackType)
    {
        switch (attackType)
        {
            case CharacterAttackType.Tackle:
                return AttackType.Tackle;

            case CharacterAttackType.Projectile:
                return AttackType.Projectile;                
        }

        throw new System.NotImplementedException();
    }
}