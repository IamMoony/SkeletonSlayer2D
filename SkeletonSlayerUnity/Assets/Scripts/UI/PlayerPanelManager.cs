using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelManager : MonoBehaviour
{
    public GameObject icon_Heart;
    public GameObject icon_Firebolt;
    public GameObject icon_Waterbolt;
    public GameObject icon_Earthbolt;

    private Character player;
    private int curHealth;
    private GameObject panel_Health;
    private GameObject panel_Spell;
    private GameObject[] hearts;


    private void Awake()
    {
        panel_Health = transform.Find("Panel_Health").gameObject;
        panel_Spell = transform.Find("Panel_Spell").gameObject;
        player = GameObject.Find("Player").GetComponent<Character>();
    }

    private void Update()
    {
        if (curHealth != player.HP_Current)
            UpdateHealth();
    }

    public void Setup()
    {
        curHealth = player.HP_Current;
        hearts = new GameObject[curHealth];
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = Instantiate(icon_Heart, panel_Health.transform);
        }
        icon_Firebolt = Instantiate(icon_Firebolt, panel_Spell.transform);
        icon_Waterbolt = Instantiate(icon_Waterbolt, panel_Spell.transform);
        icon_Earthbolt = Instantiate(icon_Earthbolt, panel_Spell.transform);
    }

    public void UpdateHealth()
    {
        curHealth = player.HP_Current;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < curHealth)
            {
                if (!hearts[i].activeSelf)
                    hearts[i].SetActive(true);
            }
            else
            {
                if (hearts[i].activeSelf)
                    hearts[i].SetActive(false);
            }
        }
    }
}
