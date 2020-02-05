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
            if (target == null)
            {
                yield return StartCoroutine(Patrol());
            }
            else
            {
                if (IsTargetInRange(true))
                {
                    yield return StartCoroutine(Attack_Melee());
                }
                else if (IsTargetInRange(false))
                {
                    yield return StartCoroutine(Attack_Ranged());
                }
                else
                    yield return StartCoroutine(GetInRange(false));
            }
        }
    }
}
