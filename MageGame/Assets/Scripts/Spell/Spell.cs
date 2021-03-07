using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string spellName;
    public string spellDescription;
    public Sprite spellIcon;
    public float spellCastTime;
    public float spellCooldown;
    public int spellInstanceLimit;
    public GameObject[] spellEffectPrefab;
    public GameObject vfxSpellCast;
    [HideInInspector] public List<GameObject> spellEffectInstances;
    [HideInInspector] public bool IsOnCooldown;

    private float currentCd;

    private void Start()
    {
        spellEffectInstances = new List<GameObject>();
    }

    private void Update()
    {
        if (currentCd > 0)
            currentCd -= Time.deltaTime;
        else if (IsOnCooldown)
            IsOnCooldown = false;
    }

    public void AddSpellEffectInstance(GameObject instance, bool cooldown)
    {
        spellEffectInstances.Add(instance);
        if (spellEffectInstances.Count > spellInstanceLimit)
            RemoveSpellEffectInstance(0);
        if (cooldown)
        {
            IsOnCooldown = true;
            currentCd = spellCooldown;
        }
    }

    private void RemoveSpellEffectInstance(int id)
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
            List<GameObject> effectsToAdd = new List<GameObject>();
            List<int> effectIDsToRemove = new List<int>();
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
                            int id = i;
                            effectIDsToRemove.Add(id);
                        }
                        //Debug.Log("Activation Effect found");
                        effectsToAdd.Add(activationEffect);
                        if (!multi)
                            break;
                    }
                    //else
                        //Debug.Log("No Activation found");
                }
                else
                {
                    effectIDsToRemove.Add(i);
                }
            }
            for (int i = 0; i < effectIDsToRemove.Count; i++)
            {
                RemoveSpellEffectInstance(effectIDsToRemove[i] - i);
            }
            for (int i = 0; i < effectsToAdd.Count; i++)
            {
                AddSpellEffectInstance(effectsToAdd[i], false);
            }
        }
    }
}
