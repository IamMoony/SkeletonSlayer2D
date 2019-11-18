using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanelManager : MonoBehaviour
{
    public GameObject characterPanel_Instance;
    public float panelOffsetY;

    private List<Transform> characterInScene;
    private List<GameObject> characterPanelInScene;
    private List<Image> characterHealthBar;
    private List<Image> characterCastBar;

    private void Awake()
    {
        characterInScene = new List<Transform>();
        characterPanelInScene = new List<GameObject>();
        characterHealthBar = new List<Image>();
        characterCastBar = new List<Image>();
    }

    public void NewPanel(Transform character)
    {
        characterInScene.Add(character);
        GameObject panel = Instantiate(characterPanel_Instance, transform);
        characterHealthBar.Add(panel.transform.Find("Bar_Health").GetChild(0).GetComponent<Image>());
        characterCastBar.Add(panel.transform.Find("Bar_Cast").GetChild(0).GetComponent<Image>());
        characterPanelInScene.Add(panel);
    }

    private void Update()
    {
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        for (int i = 0; i < characterInScene.Count; i++)
        {
            if (!characterInScene[i].GetComponent<Character>().isDead)
            {
                characterPanelInScene[i].transform.position = Camera.main.WorldToScreenPoint(characterInScene[i].position + Vector3.up * panelOffsetY);
                characterHealthBar[i].fillAmount = Mathf.Clamp((float)characterInScene[i].GetComponent<Character>().HP_Current / (float)characterInScene[i].GetComponent<Character>().HP_Base, 0, 1);
                characterCastBar[i].fillAmount = characterInScene[i].GetComponent<Character>().actionValue;
            }
            else
            {
                int indexToRemove = i;
                characterInScene.RemoveAt(indexToRemove);
                characterHealthBar.RemoveAt(indexToRemove);
                characterCastBar.RemoveAt(indexToRemove);
                GameObject panel = characterPanelInScene[indexToRemove];
                characterPanelInScene.RemoveAt(indexToRemove);
                Destroy(panel);
                i--;
            }
        }
    }
}