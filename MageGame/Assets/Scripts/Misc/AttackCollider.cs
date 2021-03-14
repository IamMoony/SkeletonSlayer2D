using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    private Character owner;

    private void Awake()
    {
        owner = transform.parent.GetComponent<Character>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Character>())
        {
            Character target = collision.GetComponent<Character>();
            if (owner is NPC)
            {
                if (target is Player)
                {
                    target.SubtractHealth(owner.DMG_Current);
                    target.Knockback(target.transform.position - owner.transform.position, owner.GetComponent<NPC>().attackKnockback_Melee);
                }
            }
        }
    }
}
