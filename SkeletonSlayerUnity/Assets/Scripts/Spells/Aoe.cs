using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aoe : SpellEffect
{
    public float effectRadius;
    public int effectDamage;
    public int effectForce;
    public LayerMask affectedLayer;
    public bool affectOwner;
    public bool nova;
    public GameObject effect_Prefab;

    private Collider2D[] targetsInRadius;

    private void Start()
    {
        Instantiate(effect_Prefab, nova ? owner.transform.position : transform.position, Quaternion.identity, transform);
        targetsInRadius = Physics2D.OverlapCircleAll(transform.position, effectRadius, affectedLayer);
        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            if (targetsInRadius[i].tag == "Character")
            {
                Character character = targetsInRadius[i].GetComponent<Character>();
                CharacterContact(character, targetsInRadius[i].ClosestPoint(transform.position));
            }
        }
        DestroyEffect();
    }

    public virtual void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        if (characterInContact != owner && !affectOwner || affectOwner)
        {
            if (owner is NPC)
                if (characterInContact is NPC)
                    return;
            characterInContact.CmdDamage(effectDamage);
            characterInContact.CmdKnockback((characterInContact.transform.position - transform.position).normalized, effectForce);
        }
    }
}
