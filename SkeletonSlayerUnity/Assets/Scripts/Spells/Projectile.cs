using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float initalVelocity;
    public Vector3 velocityModifier;
    public int contactDamage;
    public GameObject effect_Activation;
    public GameObject effect_Contact_Character_PreActivation;
    public GameObject effect_Contact_Ground_PreActivation;
    public GameObject effect_Contact_Character_PostActivation;
    public GameObject effect_Contact_Ground_PostActivation;
    public GameObject effect_Destroy;

    public bool isActivated;

    public Rigidbody2D RB;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        RB.velocity = (transform.right + velocityModifier) * initalVelocity;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Character")
        {
            CharacterContact(collider.GetComponent<Character>());
        }
        else
        {
            GroundContact(transform.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Character")
        {
            CharacterContact(collision.gameObject.GetComponent<Character>());
        }
        else
        {
            GroundContact(collision.contacts[0].point);
        }
    }

    public void ProjectileDestroy()
    {
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject, 5f);
        }
        Destroy(Instantiate(effect_Destroy, transform.position, Quaternion.identity), 5f);
        Destroy(gameObject);
    }

    public virtual void Activation()
    {
        Destroy(Instantiate(effect_Activation, transform.position, Quaternion.identity), 5f);
        isActivated = true;
    }

    public virtual void CharacterContact(Character characterInContact)
    {
        if (!isActivated)
            Destroy(Instantiate(effect_Contact_Character_PreActivation, transform.position, Quaternion.identity), 5f);
        else
            Destroy(Instantiate(effect_Contact_Character_PostActivation, transform.position, Quaternion.identity), 5f);
    }

    public virtual void GroundContact(Vector2 contactPosition)
    {
        if (!isActivated)
            Destroy(Instantiate(effect_Contact_Ground_PreActivation, contactPosition, Quaternion.identity), 5f);
        else
            Destroy(Instantiate(effect_Contact_Ground_PostActivation, contactPosition, Quaternion.identity), 5f);
    }
}
