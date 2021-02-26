using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string spellName;
    public string spellDescription;
    public Sprite spellIcon;
    public GameObject spellEffectPrefab;
    public float castTime;
    public float coolDown;

    [HideInInspector] public float cd;
    [HideInInspector] public GameObject spellInstance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (GetComponent<AudioSource>())
            audioSource = GetComponent<AudioSource>();
    }

    public void Cast(Vector2 direction, Vector2 position, Character source)
    {
        //Debug.Log("source = " + source);
        cd = coolDown;
        spellInstance = Instantiate(spellEffectPrefab, position, Quaternion.Euler(new Vector3(0, source.FacingDirection == Vector2.right ? 0 : 180, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)));
        spellInstance.GetComponent<SpellEffect>().owner = source;
        if (audioSource)
            audioSource.Play();
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
