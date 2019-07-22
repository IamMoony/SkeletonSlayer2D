using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : Projectile
{
    public float lifeTime;
    public int stunDuration;
    public int knockDownForce;
    public ContactFilter2D viableFreezeTargets;
    
    private List<Character> touchingCharacter = new List<Character>();

    public override void Start()
    {
        base.Start();
        Collider2D[] col = new Collider2D[4];
        Physics2D.OverlapCollider(GetComponent<Collider2D>(), viableFreezeTargets, col);
        for (int i = 0; i < col.Length; i++)
        {
            if (col[i] != null)
            {
                if (col[i].tag == "Character")
                    touchingCharacter.Add(col[i].GetComponent<Character>());
            }
        }
        for (int i = 0; i < touchingCharacter.Count; i++)
        {
            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = touchingCharacter[i].GetComponent<Rigidbody2D>();
        }
        if (touchingCharacter.Count > 0)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    public override void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        base.CharacterContact(characterInContact, contactPosition);
        if (RB.velocity.magnitude > 0.5f)
        {
            characterInContact.Damage(contactDamage);
            characterInContact.Knockback(Vector2.down, knockDownForce);
            characterInContact.Stun(true, stunDuration);
            ProjectileDestroy();
        }
    }

    public override void GroundContact(Vector2 contactPosition)
    {
        //base.GroundContact(contactPosition);
    }
}
