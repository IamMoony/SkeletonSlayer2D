using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergingRock : Projectile
{
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

    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        if (emerging)
        {
            characterInContact.CmdDamage(contactDamage);
            characterInContact.CmdKnockback(((Vector2)characterInContact.transform.position - contactPosition).normalized, contactKnockBackForce);
        }
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
