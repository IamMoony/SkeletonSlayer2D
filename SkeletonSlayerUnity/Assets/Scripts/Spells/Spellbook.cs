using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellbook : MonoBehaviour
{
    public string spellName;
    public string spellDescription;
    public Sprite spellIcon;
    public GameObject spellPrefab;
    public float castTime;
    public float coolDown;

    [HideInInspector] public float cd;
    [HideInInspector] public GameObject spellInstance;

    public void Cast(Vector2 direction, Vector2 position, Character source)
    {
        //Debug.Log("source = " + source);
        cd = coolDown;
        spellInstance = Instantiate(spellPrefab, position, Quaternion.Euler(new Vector3(0, source.FacingDirection == Vector2.right ? 0 : 180, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)));
        spellInstance.GetComponent<Spell>().owner = source;
    }

    public void Activate(Vector2 activationDirection)
    {
        if (spellInstance)
        {
            if (spellInstance.GetComponent<Projectile>())
                if (!spellInstance.GetComponent<Projectile>().isActivated)
                    spellInstance.GetComponent<Projectile>().Activation(activationDirection);
        }
    }
}
