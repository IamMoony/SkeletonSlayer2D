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

    private GameObject spellInstance;

    public void Cast(Vector2 position, Character source)
    {
        cd = coolDown;
        if (spellInstance)
            spellInstance.GetComponent<Projectile>().ProjectileDestroy();
        spellInstance = Instantiate(spellPrefab, position, Quaternion.Euler(source.FacingDirection == (Vector2)transform.right ? 0 : 180, 0, source.FacingDirection == (Vector2)transform.right ? 0 : 180));
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

    private void Update()
    {
        if (cd > 0)
        {
            cd -= Time.deltaTime;
        }
    }
}