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

    public override void GroundContact()
    {
        base.GroundContact();
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
                targetsInRadius[i].GetComponent<Character>().Burn(true);
                targetsInRadius[i].GetComponent<Character>().Damage(explosionDamage, Vector2.up);
                targetsInRadius[i].GetComponent<Rigidbody2D>().AddForce((targetsInRadius[i].transform.position - transform.position).normalized * explosionForce);
            }
        }
    }
}
