using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosion : Aoe
{
    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        base.CharacterContact(characterInContact, contactPosition);
        characterInContact.CmdBurn(true);
    }
}
