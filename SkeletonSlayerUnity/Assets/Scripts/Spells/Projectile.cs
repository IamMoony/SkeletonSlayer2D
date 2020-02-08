using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    public float initalVelocity;
    public Vector3 velocityModifier;
    public int contactDamage;
    public float lifeTime = 10f;
    public GameObject effect_Activation;
    public GameObject effect_Contact_Character_PreActivation;
    public GameObject effect_Contact_Ground_PreActivation;
    public GameObject effect_Contact_Character_PostActivation;
    public GameObject effect_Contact_Ground_PostActivation;
    public GameObject effect_Destroy;

    public bool isActivated;
    public Character owner;

    public Rigidbody2D RB;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    public virtual void Start()
    {
        RB.velocity = (transform.right + velocityModifier) * initalVelocity;
    }

    private void Update()
    {
        if (lifeTime > 0)
            lifeTime -= Time.deltaTime;
        else
            ProjectileDestroy();
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

    public void ProjectileDestroy()
    {
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            Destroy(child.gameObject, 1f);
            child.SetParent(null);
        }
        Destroy(Instantiate(effect_Destroy, transform.position, Quaternion.identity), 5f);
        NetworkServer.Destroy(gameObject);
    }

    public virtual void Activation(Vector2 direction)
    {
        Destroy(Instantiate(effect_Activation, transform.position, Quaternion.identity), 5f);
        isActivated = true;
    }

    public virtual void CharacterContact(Character characterInContact, Vector2 contactPosition)
    {
        characterInContact.CmdDamage(contactDamage);
        if (!isActivated)
            Destroy(Instantiate(effect_Contact_Character_PreActivation, contactPosition, Quaternion.identity), 5f);
        else
            Destroy(Instantiate(effect_Contact_Character_PostActivation, contactPosition, Quaternion.identity), 5f);
    }

    public virtual void GroundContact(Vector2 contactPosition)
    {
        if (!isActivated)
            Destroy(Instantiate(effect_Contact_Ground_PreActivation, contactPosition, Quaternion.identity), 5f);
        else
            Destroy(Instantiate(effect_Contact_Ground_PostActivation, contactPosition, Quaternion.identity), 5f);
    }
}
