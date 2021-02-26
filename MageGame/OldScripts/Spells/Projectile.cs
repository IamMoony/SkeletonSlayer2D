using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : SpellEffect
{
    public float initalVelocity;
    public Vector3 velocityModifier;
    public int contactDamage;
    public int contactKnockBackForce;
    public float lifeTime = 10f;
    public GameObject effect_Activation;
    public GameObject effect_Contact_Character_PreActivation;
    public GameObject effect_Contact_Ground_PreActivation;
    public GameObject effect_Contact_Character_PostActivation;
    public GameObject effect_Contact_Ground_PostActivation;

    public bool isActivated;

    public Rigidbody2D RB;

    public override void Awake()
    {
        base.Awake();
        RB = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        RB.velocity = (transform.right + velocityModifier) * initalVelocity;
    }

    public virtual void Update()
    {
        if (lifeTime > 0)
            lifeTime -= Time.deltaTime;
        else
            DestroyEffect();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Character")
        {
            CharacterContact(collider.GetComponent<Character>(), collider.ClosestPoint(transform.position));
        }
        else if (collider.tag == "Ground")
        {
            GroundContact(collider.ClosestPoint(transform.position));
        }
    }

    public virtual void Activation(Vector2 direction)
    {
        Destroy(Instantiate(effect_Activation, transform.position, Quaternion.identity), 5f);
        isActivated = true;
    }

    public virtual void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        characterInContact.CmdDamage(contactDamage);
        characterInContact.CmdKnockback(((Vector2)characterInContact.transform.position - contactPosition).normalized, contactKnockBackForce);
        if (!isActivated)
        {
            audioSource.Play();
            Destroy(Instantiate(effect_Contact_Character_PreActivation, contactPosition, Quaternion.identity), 5f);
        }
        else
        {
            Destroy(Instantiate(effect_Contact_Character_PostActivation, contactPosition, Quaternion.identity), 5f);
        }
    }

    public virtual void GroundContact(Vector2 contactPosition)
    {
        if (!isActivated)
        {
            audioSource.Play();
            Destroy(Instantiate(effect_Contact_Ground_PreActivation, contactPosition, Quaternion.identity), 5f);
        }
        else
        {
            Destroy(Instantiate(effect_Contact_Ground_PostActivation, contactPosition, Quaternion.identity), 5f);
        }
    }
}
