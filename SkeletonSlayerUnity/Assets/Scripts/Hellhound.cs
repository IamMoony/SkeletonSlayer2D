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
                if (!targetInRange)
                    yield return StartCoroutine(Pursue());
                else
                    yield return StartCoroutine(Attack_Melee());
            }
        }
    }
}
