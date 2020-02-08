using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hellhound : NPC
{
    private void Start()
    {
        if (isClientOnly)
            return;
        StartCoroutine(Behavior());
    }

    IEnumerator Behavior()
    {
        while (true)
        {
            if (projectile)
            {
                Debug.Log("ThreatCheck: " + ProjectileThreatCheck());
                if (ProjectileThreatCheck())
                    yield return StartCoroutine(Evade());
            }
            if (target == null)
            {
                yield return StartCoroutine(Patrol());
            }
            else if (target)
            {
                if (IsTargetInRange(true) && cooldown_Melee <= 0)
                {
                    yield return StartCoroutine(Attack_Melee());
                }
                else if (IsTargetInRange(false) && cooldown_Ranged <= 0)
                {
                    yield return StartCoroutine(Attack_Ranged());
                }
                else
                    yield return StartCoroutine(GetInRange(true));
            }
        }
    }
}
