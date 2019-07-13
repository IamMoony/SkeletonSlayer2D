using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Player.Element element;
    public int dmg;
    public GameObject effect_Ground;
    public GameObject effect_Destroy;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Character")
        {
            collision.GetComponent<Character>().Damage(dmg, transform.right);
            if (element == Player.Element.Fire)
            {
                collision.GetComponent<Character>().Burn(true);
            }
            else if (element == Player.Element.Water)
            {
                collision.GetComponent<Character>().Freeze(true);
            }
        }
        ProjectileDestroy();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Character")
        {
            collision.gameObject.GetComponent<Character>().Damage(dmg, transform.right);
            collision.gameObject.GetComponent<Character>().Root(true);
            ProjectileDestroy();
        }
    }

    void ProjectileDestroy()
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

    void GroundEffect()
    {
        Instantiate(effect_Ground, transform.position, Quaternion.identity);
    }
}
