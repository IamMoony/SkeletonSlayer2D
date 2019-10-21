using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hellhound : NPC
{
    private void Start()
    {
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
                if (TargetInRange(true))
                    yield return StartCoroutine(Attack_Melee());
                else if (TargetInRange(false))
                    yield return StartCoroutine(Cast_Spell());
                else
                    yield return StartCoroutine(GetInRange(false));
            }
        }
    }
}
