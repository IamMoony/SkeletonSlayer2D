using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterbolt : Projectile
{
    public GameObject iceBlock;

    public override void Activation(Vector2 direction)
    {
        base.Activation(direction);
        CreateIceBlock();
    }

    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        base.CharacterContact(characterInContact, contactPosition);
        if (!isActivated)
        {
            characterInContact.Wet(true);
        }
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        base.GroundContact(contactPosition);
        if (!isActivated)
        {
            ProjectileDestroy();
        }
    }

    private void CreateIceBlock()
    {
        Instantiate(iceBlock, transform.position, Quaternion.identity);
        ProjectileDestroy();
    }
}
