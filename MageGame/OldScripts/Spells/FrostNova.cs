using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostNova : Aoe
{
    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        base.CharacterContact(characterInContact, contactPosition);
        characterInContact.Freeze(true);
    }
}
