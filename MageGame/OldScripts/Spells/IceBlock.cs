﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : Projectile
{
    public int stunDuration;
    public int knockDownForce;

    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        base.CharacterContact(characterInContact, contactPosition);
        if (RB.velocity.magnitude > 0.1f)
        {
            characterInContact.CmdDamage(contactDamage);
            characterInContact.CmdKnockback(Vector2.down, knockDownForce);
            characterInContact.Stun(true, stunDuration);
            DestroyEffect();
        }
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        base.GroundContact(contactPosition);
    }
}
