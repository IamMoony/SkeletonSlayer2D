using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellbook : MonoBehaviour
{
    public string bookName;
    public string bookDescription;

    public GameObject primarySpellInstance;
    public GameObject[] secondarySpellInstance;

    [HideInInspector] public Spell primarySpell;
    [HideInInspector] public Spell[] secondarySpell;

    private void Awake()
    {
        primarySpellInstance = Instantiate(primarySpellInstance, transform);
        primarySpell = primarySpellInstance.GetComponent<Spell>();
        secondarySpell = new Spell[secondarySpellInstance.Length];
        for (int i = 0; i < secondarySpellInstance.Length; i++)
        {
            secondarySpellInstance[i] = Instantiate(secondarySpellInstance[i], transform);
            secondarySpell[i] = secondarySpellInstance[i].GetComponent<Spell>();
        }
    }

    private void Update()
    {
        if (primarySpell.cd > 0)
            primarySpell.cd -= Time.deltaTime;
        for (int i = 0; i < secondarySpell.Length; i++)
        {
            if (secondarySpell[i].cd > 0)
                secondarySpell[i].cd -= Time.deltaTime;
        }
    }
}
