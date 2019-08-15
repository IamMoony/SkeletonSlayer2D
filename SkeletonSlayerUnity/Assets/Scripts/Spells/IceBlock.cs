using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : Projectile
{
    public float lifeTime;
    public int stunDuration;
    public int knockDownForce;

    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        base.CharacterContact(characterInContact, contactPosition);
        if (RB.velocity.magnitude > 0.5f)
        {
            characterInContact.Damage(contactDamage);
            characterInContact.Knockback(Vector2.down, knockDownForce);
            characterInContact.Stun(true, stunDuration);
            ProjectileDestroy();
        }
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        base.GroundContact(contactPosition);
    }
}
