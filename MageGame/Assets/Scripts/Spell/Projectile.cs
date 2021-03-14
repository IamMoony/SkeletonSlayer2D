using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : SpellEffect
{
    public float initialVelocity;
    public float power;
    public float knockBackForce;
    public int activationLimit;
    public GameObject spellEffectOnActivation;
    public GameObject vfxContactCharacter;
    public GameObject vfxContactGround;
    public GameObject vfxActivation;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Vector2 targetPosition;

    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        rb.velocity = transform.right * initialVelocity;
    }

    private void Update()
    {
        if (targetPosition != Vector2.zero)
        {
            if (Vector2.Distance(transform.position, targetPosition) <= 0.05f)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<Character>())
        {
            CharacterContact(collider.GetComponent<Character>(), collider.ClosestPoint(transform.position));
        }
        else if (collider.tag == "Ground")
        {
            GroundContact(collider.ClosestPoint(transform.position));
        }
    }

    public virtual GameObject Activation(Vector2 direction)
    {
        if (activationLimit > 0 && spellEffectOnActivation != null)
        {
            if (vfxActivation)
                Destroy(Instantiate(vfxActivation, transform.position, Quaternion.identity), 1f);
            GameObject effect = Instantiate(spellEffectOnActivation, transform.position, Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.x, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)));
            effect.GetComponent<SpellEffect>().owner = owner;
            activationLimit--;
            return effect;
        }
        else
            return null;
    }

    public virtual void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        characterInContact.SubtractHealth(Mathf.RoundToInt(owner.DMG_Current * power));
        Vector2 knockDir = ((Vector2)characterInContact.transform.position - contactPosition).normalized;
        characterInContact.Knockback(new Vector2(knockDir.x, 1f), knockBackForce);
        characterInContact.Float();
        if (vfxContactCharacter)
            Destroy(Instantiate(vfxContactCharacter, contactPosition, Quaternion.identity), 1f);
    }

    public virtual void GroundContact(Vector2 contactPosition)
    {
        if (vfxContactGround)
            Destroy(Instantiate(vfxContactGround, contactPosition, Quaternion.identity), 1f);
    }
}
