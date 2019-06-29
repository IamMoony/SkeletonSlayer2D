using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public enum projectileType {Normal, Lob, Ground};
    public projectileType type;
    public enum projectileElement {Fire, Ice, Earth};
    public projectileElement element;
    public int dmg;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == projectileType.Normal)
        {
            if (element == projectileElement.Fire)
            {
                if (collision.gameObject.tag == "Character")
                {
                    
                }
            }
            else if (element == projectileElement.Ice)
            {

            }
            else if (element == projectileElement.Earth)
            {

            }
        }
        else if (type == projectileType.Lob)
        {
            if (element == projectileElement.Fire)
            {

            }
            else if (element == projectileElement.Ice)
            {

            }
            else if (element == projectileElement.Earth)
            {

            }
        }
        else if (type == projectileType.Ground)
        {
            if (element == projectileElement.Fire)
            {

            }
            else if (element == projectileElement.Ice)
            {

            }
            else if (element == projectileElement.Earth)
            {

            }
        }
    }
}
