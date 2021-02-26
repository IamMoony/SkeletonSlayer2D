using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public int dmgAmount;
    public int knockbackForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Hit");
        collision.GetComponent<Character>().CmdDamage(dmgAmount);
        collision.GetComponent<Character>().CmdKnockback(transform.parent.GetComponent<Character>().FacingDirection, knockbackForce);
    }
}
