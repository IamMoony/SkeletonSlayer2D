using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string spellName;
    public string spellDescription;
    public Sprite spellIcon;
    public float spellCastTime;
    public int spellInstanceLimit;
    public GameObject[] spellEffectPrefab;
    public GameObject vfxSpellCast;

    public List<GameObject> spellEffectInstances;

    private void Start()
    {
        spellEffectInstances = new List<GameObject>();
    }

    public void AddSpellEffectInstance(GameObject instance)
    {
        spellEffectInstances.Add(instance);
        if (spellEffectInstances.Count > spellInstanceLimit)
            RemoveSpellEffect(0);
    }

    private void RemoveSpellEffect(int id)
    {
        if (spellEffectInstances[id])
            spellEffectInstances[id].GetComponent<SpellEffect>().DestroySelf();
        spellEffectInstances.RemoveAt(id);
    }

    public void Activate(Vector2 target, bool multi)
    {
        if (spellEffectInstances.Count > 0)
        {
            //Debug.Log("Activating with " + spellEffectInstances.Count + " SpellEffectInstances");
            List<GameObject> effects = new List<GameObject>();
            for (int i = 0; i < spellEffectInstances.Count; i++)
            {
                //Debug.Log("SpellEffect " + i);
                if (spellEffectInstances[i])
                {
                    //Debug.Log("Instance found");
                    GameObject activationEffect = spellEffectInstances[i].GetComponent<Projectile>().Activation(target - (Vector2)spellEffectInstances[i].transform.position);
                    if (activationEffect)
                    {
                        if (spellEffectInstances[i].GetComponent<Projectile>().activationLimit == 0)
                        {
                            RemoveSpellEffect(i);
                            i--;
                        }
                        //Debug.Log("Activation Effect found");
                        effects.Add(activationEffect);
                        if (!multi)
                            break;
                    }
                }
                else
                {
                    RemoveSpellEffect(i);
                    i--;
                }
            }
            for (int i = 0; i < effects.Count; i++)
            {
                //Debug.Log("Adding Activation Effect " + i);
                AddSpellEffectInstance(effects[i]);
            }
        }
    }
}
