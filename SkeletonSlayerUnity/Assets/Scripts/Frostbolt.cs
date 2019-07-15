using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frostbolt : Projectile
{
    public int freezingDamage;
    public float frozenRockGravity;
    public int frozenRockDamage;
    public int frozenRockStunDuration;

    private Character engulfedCharacter;

    public override void Activation()
    {
        base.Activation();
        if (engulfedCharacter != null)
        {
            engulfedCharacter.Freeze(true);
            engulfedCharacter.Damage(freezingDamage, Vector2.down);
            ProjectileDestroy();
        }
        else
        {
            FrozenRock();
        }
    }

    public override void CharacterContact(Character characterInContact)
    {
        base.CharacterContact(characterInContact);
        if (!isActivated)
        {
            characterInContact.Wet(true);
            engulfedCharacter = characterInContact;
            RB.velocity = Vector2.zero;
            transform.Find("Effect_WaterBolt").gameObject.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            characterInContact.Damage(frozenRockDamage, Vector2.down);
            //characterInContact.Stun(true, frozenRockStunDuration);
            ProjectileDestroy();
        }
    }

    public override void GroundContact()
    {
        base.GroundContact();
        if (isActivated)
        {
            Destroy(gameObject, 5f);
        }
        else
        {
            ProjectileDestroy();
        }
    }

    private void FrozenRock()
    {
        transform.Find("Effect_WaterBolt").gameObject.SetActive(false);
        GetComponent<SpriteRenderer>().enabled = true;
        RB.gravityScale = frozenRockGravity;
    }
}
