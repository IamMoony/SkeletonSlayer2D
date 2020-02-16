using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostNova : Spell
{
    public int damage;
    public float radius;
    public int knockBack;
    public GameObject effect_Prefab;
    public LayerMask layerAffected;

    private GameObject effect_Instance;

    void Start()
    {
        effect_Instance = Instantiate(effect_Prefab, owner.transform.position, Quaternion.identity, transform);
        Collider2D[] targetsInRadius = Physics2D.OverlapCircleAll(transform.position, radius, layerAffected);
        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            if (targetsInRadius[i].tag == "Character" && targetsInRadius[i].GetComponent<Character>() != owner)
            {
                Character character = targetsInRadius[i].GetComponent<Character>();
                character.Freeze(true);
                character.CmdDamage(damage);
                character.CmdKnockback((targetsInRadius[i].transform.position - transform.position).normalized, knockBack);
            }
        }
        Destroy(this, 1f);
    }
}
