using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string spellName;
    public string spellDescription;
    public Sprite spellIcon;
    public GameObject spellPrefab;
    public float castTime;
    public float coolDown;

    public float cd;
    public GameObject spellInstance;

    public void Cast(Vector2 direction, Vector2 position, Character source)
    {
        Debug.Log("source = " + source);
        cd = coolDown;
        spellInstance = Instantiate(spellPrefab, position, Quaternion.Euler(direction == (Vector2)transform.right ? 0 : 180, 0, direction == (Vector2)transform.right ? 0 : 180));
        spellInstance.GetComponent<Projectile>().owner = source;
    }

    public void Activate(Vector2 activationDirection)
    {
        if (spellInstance)
        {
            if (!spellInstance.GetComponent<Projectile>().isActivated)
                spellInstance.GetComponent<Projectile>().Activation(activationDirection);
        }
    }
}