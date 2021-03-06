﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterbolt : Projectile
{
    public float freezeRadius;

    public GameObject iceBlock;

    public override void Activation(Vector2 direction)
    {
        base.Activation(direction);
        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, freezeRadius);
        int frozen = 0;
        foreach (Collider2D target in targetsInRange)
        {
            if (target.tag == "Character")
            {
                target.GetComponent<Character>().Freeze(true);
                frozen++;
            }
        }
        if (frozen == 0)
        {
            iceBlock.GetComponent<SpellEffect>().owner = owner;
            Instantiate(iceBlock, transform.position, Quaternion.identity);
            DestroyEffect();
        }
        else
            DestroyEffect();
    }

    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        if (!isActivated)
        {
            if (characterInContact != owner)
            {
                base.CharacterContact(characterInContact, contactPosition);
                characterInContact.Wet(true);
            }
            else
            {
                //characterInContact.rb.gravityScale = 0;
            }
        }
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        base.GroundContact(contactPosition);
        if (!isActivated)
        {
            DestroyEffect();
        }
    }
    /*
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Character")
        {
            if (collider.GetComponent<Character>() == owner)
            {
                collider.GetComponent<Character>().rb.gravityScale = 1;
            }
        }
    }
    */
}
