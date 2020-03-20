using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSelection : MonoBehaviour
{
    public GameObject panel_Spellbook;
    public GameObject panel_ActiveSpells;
    public GameObject button_StartBattle;
    public GameObject button_Prefab;

    [HideInInspector] public Character localPlayer;

    private GameObject[] button_Spellbook;
    private List<GameObject> button_Selection;

    private void Awake()
    {
        panel_Spellbook.SetActive(false);
        panel_ActiveSpells.SetActive(false);
        button_StartBattle.SetActive(false);
        button_Selection = new List<GameObject>();
    }

    public void StartSelection()
    {
        Time.timeScale = 0;
        panel_Spellbook.SetActive(true);
        panel_ActiveSpells.SetActive(true);
        button_StartBattle.SetActive(true);
        FillSpellbook();
    }

    public void FinishSelection()
    {
        localPlayer.GetComponent<Player>().activeSpellID = new List<int>();
        for (int i = 0; i < button_Selection.Count; i++)
        {
            localPlayer.GetComponent<Player>().activeSpellID.Add(button_Selection[i].GetComponent<SpellIdentity>().ID);
        }
        ClearSelection();
        panel_Spellbook.SetActive(false);
        panel_ActiveSpells.SetActive(false);
        button_StartBattle.SetActive(false);
        Time.timeScale = 1;
    }

    public void FillSpellbook()
    {
        button_Spellbook = new GameObject[localPlayer.spellBook.secondarySpell.Length];
        for (int i = 0; i < localPlayer.spellBook.secondarySpell.Length; i++)
        {
            int id = i;
            button_Spellbook[i] = Instantiate(button_Prefab, panel_Spellbook.transform);
            button_Spellbook[i].GetComponent<Button>().onClick.AddListener(delegate { AddSpellToActive(id); });
            button_Spellbook[i].GetComponentInChildren<Text>().text = localPlayer.spellBook.secondarySpell[i].spellName;
        }
    }

    private void ClearSelection()
    {
        for (int i = 0; i < button_Selection.Count; i++)
        {
            Destroy(button_Selection[i]);
        }
        button_Selection.Clear();
    }

    public void AddSpellToActive(int id)
    {
        GameObject button = Instantiate(button_Prefab, panel_ActiveSpells.transform);
        button.GetComponent<Button>().onClick.AddListener(delegate { RemoveSpellFromSelection(button); });
        button.GetComponentInChildren<Text>().text = localPlayer.spellBook.secondarySpell[id].spellName;
        button.GetComponent<SpellIdentity>().ID = id;
        button_Selection.Add(button);
    }

    public void RemoveSpellFromSelection(GameObject button)
    {
        button_Selection.Remove(button);
        Destroy(button);
    }
}
