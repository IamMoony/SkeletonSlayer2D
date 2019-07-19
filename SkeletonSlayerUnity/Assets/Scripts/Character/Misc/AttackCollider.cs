using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public int dmgAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Character>().Damage(dmgAmount, transform.parent.GetComponent<Character>().FacingDirection);
    }
}
