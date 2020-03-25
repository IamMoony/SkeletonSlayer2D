using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthbolt : Projectile
{
    public float burrowVelocity;
    public float burrowDepth;
    public GameObject emergingRock;
    public LayerMask groundLayer;
    public float rockOffset;

    public override void Activation(Vector2 direction)
    {
        base.Activation(direction);
        emergingRock.GetComponent<SpellEffect>().owner = owner;
        Instantiate(emergingRock, transform.position + -transform.up * rockOffset, transform.rotation);
        DestroyEffect();
    }

    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        if (characterInContact != owner)
        {
            base.CharacterContact(characterInContact, contactPosition);
            if (!isActivated)
            {
                characterInContact.Root(true);
            }
            DestroyEffect();
        }
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        base.GroundContact(contactPosition);
        if (!isActivated)
        {
            StartCoroutine(Burrow());
        }
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
        if (targetDir != Vector2.down)
        {
            transform.Rotate(new Vector3(0, 0, Vector3.Distance(targetDir, Vector3.right) < 0.1f ? 90 : -90));
        }
        while (!isActivated)
        {
            if (isDirectionObstructed(transform.right))
            {
                RB.velocity = transform.right * burrowVelocity;
            }
            else if (isDirectionObstructed(-transform.up))
            {
                transform.Rotate(new Vector3(0, 0, -90));
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, 90));
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
