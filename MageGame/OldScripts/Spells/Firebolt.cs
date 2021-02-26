using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firebolt : Projectile
{
    public bool projectileBurn;
    public bool projectileBounce;
    public float chargeVelocity;

    public GameObject fireExplosion;

    public override void Activation(Vector2 direction)
    {
        base.Activation(direction);
        RB.velocity = Vector2.zero;
        RB.gravityScale = 0;
        RB.velocity = direction * chargeVelocity;
        fireExplosion.GetComponent<SpellEffect>().owner = owner;
    }

    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        if (characterInContact != owner)
        {
            if (owner is NPC)
                if (characterInContact is NPC)
                    return;
            base.CharacterContact(characterInContact, contactPosition);
            if (!isActivated)
            {
                if (projectileBurn)
                    characterInContact.CmdBurn(true);
            }
            else
            {
                Instantiate(fireExplosion, transform.position, Quaternion.identity);
            }
            DestroyEffect();
        }
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        base.GroundContact(contactPosition);
        if (isActivated)
        {
            Instantiate(fireExplosion, transform.position, Quaternion.identity);
            DestroyEffect();
        }
        else
        {
            if (projectileBounce)
            {
                RaycastHit2D ground = Physics2D.Raycast(transform.position, RB.velocity.normalized);
                RB.velocity = Vector2.Reflect(RB.velocity, ground.normal);
            }
            else
            {
                DestroyEffect();
            }
        }
    }
}
