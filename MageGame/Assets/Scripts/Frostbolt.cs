using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frostbolt : Projectile
{
    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        if (characterInContact != owner)
        {
            base.CharacterContact(characterInContact, contactPosition);
            DestroySelf();
        }
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        DestroySelf();
    }
}
