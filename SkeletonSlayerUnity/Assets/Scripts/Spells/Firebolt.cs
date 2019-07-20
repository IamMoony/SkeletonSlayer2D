using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : Projectile
{
    public float chargeVelocity;
    public float explosionRadius;
    public int explosionForce;
    public int explosionDamage;
    public bool explosionBurn;
    public LayerMask layerAffectedByExplosion;

    public override void Activation()
    {
        base.Activation();
        RB.velocity = Vector2.zero;
        RB.gravityScale = 0;
        RB.velocity = transform.right * chargeVelocity;
    }

    public override void CharacterContact(Character characterInContact)
    {
        base.CharacterContact(characterInContact);
        if (!isActivated)
        {
            characterInContact.Burn(true);
        }
        else
        {
            Explode();
        }
        ProjectileDestroy();
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        base.GroundContact(contactPosition);
        if (isActivated)
        {
            Explode();
            ProjectileDestroy();
        }
    }

    private void Explode()
    {
        Collider2D[] targetsInRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius, layerAffectedByExplosion);
        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            if (targetsInRadius[i].tag == "Character")
            {
                Character character = targetsInRadius[i].GetComponent<Character>();
                if (explosionBurn)
                    character.Burn(true);
                character.Damage(explosionDamage);
                character.Knockback((targetsInRadius[i].transform.position - transform.position).normalized,  explosionForce);
            }
        }
    }
}
