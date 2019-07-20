using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public int dmgAmount;
    public int knockbackForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Character>().Damage(dmgAmount);
        collision.GetComponent<Character>().Knockback(transform.parent.GetComponent<Character>().FacingDirection, knockbackForce);
    }
}
