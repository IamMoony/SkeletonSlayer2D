using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergingRock : Projectile
{
    public float lifeTime;
    public int knockBackForce;
    public float emergeAmount;
    public float emergeSpeed;

    private bool emerging;
    private Vector2 emergeDirection;

    public override void Start()
    {
        base.Start();
        Destroy(gameObject, lifeTime);
        StartCoroutine(Emerge());
    }

    public override void CharacterContact(Character characterInContact)
    {
        base.CharacterContact(characterInContact);
        if (emerging)
        {
            characterInContact.Damage(contactDamage);
            characterInContact.Knockback(emergeDirection, knockBackForce);
        }
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        //base.GroundContact(contactPosition);
    }

    IEnumerator Emerge()
    {
        emerging = true;
        emergeDirection = transform.up;
        Vector2 targetPos = (Vector2)transform.position + emergeDirection * emergeAmount;
        RB.velocity = emergeDirection * emergeSpeed;
        while (Vector2.Distance(transform.position, targetPos) > 0.05f)
        {
            yield return new WaitForEndOfFrame();
        }
        RB.velocity = Vector2.zero;
        emerging = false;
    }
}
