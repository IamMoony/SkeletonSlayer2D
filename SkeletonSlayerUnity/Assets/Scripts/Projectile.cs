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
    public LayerMask ignoredLayer;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Projectile Trigger Enter");
        if (type == projectileType.Normal)
        {
            if (element == projectileElement.Fire)
            {
                if (collision.gameObject.layer != ignoredLayer)
                {
                    DelayedChildDestroy();
                    Destroy(gameObject, 0.1f);
                }
            }
            else if (element == projectileElement.Ice)
            {
                if (collision.gameObject.layer != ignoredLayer)
                {
                    DelayedChildDestroy();
                    Destroy(gameObject, 0.1f);
                }
            }
            else if (element == projectileElement.Earth)
            {
                if (collision.gameObject.layer != ignoredLayer)
                {
                    DelayedChildDestroy();
                    Destroy(gameObject, 0.1f);
                }
            }
        }
        else if (type == projectileType.Lob)
        {
            if (element == projectileElement.Fire)
            {
                if (collision.gameObject.layer != ignoredLayer)
                {
                    DelayedChildDestroy();
                    Destroy(gameObject, 0.1f);
                }
            }
            else if (element == projectileElement.Ice)
            {
                if (collision.gameObject.layer != ignoredLayer)
                {
                    DelayedChildDestroy();
                    Destroy(gameObject, 0.1f);
                }
            }
            else if (element == projectileElement.Earth)
            {
                if (collision.gameObject.layer != ignoredLayer)
                {
                    DelayedChildDestroy();
                    Destroy(gameObject, 0.1f);
                }
            }
        }
        else if (type == projectileType.Ground)
        {
            if (element == projectileElement.Fire)
            {
                if (collision.gameObject.layer != ignoredLayer)
                {
                    DelayedChildDestroy();
                    Destroy(gameObject, 0.1f);
                }
            }
            else if (element == projectileElement.Ice)
            {
                if (collision.gameObject.layer != ignoredLayer)
                {
                    DelayedChildDestroy();
                    Destroy(gameObject, 0.1f);
                }
            }
            else if (element == projectileElement.Earth)
            {
                if (collision.gameObject.layer != ignoredLayer)
                {
                    DelayedChildDestroy();
                    Destroy(gameObject, 0.1f);
                }
            }
        }
    }

    void DelayedChildDestroy()
    {
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.SetParent(null);
            Destroy(child, 5f);
        }
    }
}
