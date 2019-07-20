﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthbolt : Projectile
{
    public float burrowVelocity;
    public float burrowDepth;
    public GameObject emergingRock;
    public LayerMask groundLayer;

    public override void Activation()
    {
        base.Activation();
        EmergingRock();
    }

    public override void CharacterContact(Character characterInContact)
    {
        base.CharacterContact(characterInContact);
        if (!isActivated)
        {
            characterInContact.Root(true);
        }
        ProjectileDestroy();
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        base.GroundContact(contactPosition);
        if (!isActivated)
        {
            StartCoroutine(Burrow());
        }
    }

    void EmergingRock()
    {
        Instantiate(emergingRock, transform.position, Quaternion.Euler(transform.up * (isDirectionObstructed(transform.up) ? -1 : 1)));
        ProjectileDestroy();
    }

    IEnumerator Burrow()
    {
        RB.gravityScale = 0;
        RB.velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Vector2 targetDir = isDirectionObstructed(transform.right) ? transform.right : Vector3.down;
        Vector2 targetPos = (Vector2)transform.position + targetDir * burrowDepth;
        while (Vector2.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * burrowVelocity);
            yield return new WaitForEndOfFrame();
        }
        while (!isActivated)
        {
            if (isDirectionObstructed(transform.right))
            {
                RB.velocity = transform.right * burrowVelocity;
            }
            else if (isDirectionObstructed(transform.up))
            {
                transform.Rotate(new Vector3(0, 0, 90));
            }
            else if (isDirectionObstructed(-transform.up))
            {
                transform.Rotate(new Vector3(0, 0, -90));
            }
            yield return new WaitForEndOfFrame();
        }
    }

    bool isDirectionObstructed(Vector2 direction)
    {
        if (Physics2D.OverlapCircle((Vector2)transform.position + direction * 0.2f, 0.01f, groundLayer))
        {
            return true;
        }
        return false;
    }
}