using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hellhound : NPC
{
    public override void Start()
    {
        base.Start();
        StartCoroutine(Behavior());
    }

    IEnumerator Behavior()
    {
        while (true)
        {
            if (foreignProjectile)
            {
                Debug.Log("ThreatCheck: " + ProjectileThreatCheck());
                if (ProjectileThreatCheck())
                    yield return StartCoroutine(JumpEvasion());
            }
            if (target == null)
            {
                yield return StartCoroutine(Patrol());
            }
            else if (target)
            {
                if (IsInMeleeRange() && cooldown_Melee <= 0)
                {
                    yield return StartCoroutine(Routine_AttackMelee(true));
                }
                else if (IsInRangedRange() && cooldown_Ranged <= 0 && projectile_Ranged && !IsInMeleeRange())
                {
                    yield return StartCoroutine(Routine_AttackRanged());
                }
                else
                {
                    yield return StartCoroutine(GetInRange(true));
                }
            }
        }
    }

    private IEnumerator JumpEvasion()
    {
        //Debug.Log("Evasion");
        Vector2 evadeDir = Vector2.up;
        Jump(evadeDir, 0.75f);
        foreignProjectile = null;
        yield return new WaitForSeconds(.5f);
    }
}
