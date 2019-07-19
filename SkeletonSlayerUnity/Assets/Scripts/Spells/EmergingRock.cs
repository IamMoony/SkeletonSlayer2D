using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergingRock : Projectile
{
    public int knockBackForce;
    public float emergeAmount;
    public float emergeSpeed;

    public override void Start()
    {
        base.Start();
        StartCoroutine(Emerge());
    }

    public override void CharacterContact(Character characterInContact)
    {
        base.CharacterContact(characterInContact);
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        //base.GroundContact(contactPosition);
    }

    IEnumerator Emerge()
    {
        Vector2 targetPos = transform.position + transform.up * emergeAmount;
        RB.velocity = transform.up * emergeSpeed;
        while (Vector2.Distance(transform.position, targetPos) > 0.05f)
        {
            yield return new WaitForEndOfFrame();
        }
        RB.velocity = Vector2.zero;
    }
}
