using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterbolt : Projectile
{
    public GameObject iceBlock;

    public override void Activation()
    {
        base.Activation();
        CreateIceBlock();
    }

    public override void CharacterContact(Character characterInContact)
    {
        base.CharacterContact(characterInContact);
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
