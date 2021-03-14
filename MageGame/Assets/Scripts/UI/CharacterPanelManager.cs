using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanelManager : MonoBehaviour
{
    public GameObject characterPanel_Instance;

    private Transform characterHolder;
    private List<Transform> characterInScene;
    private List<GameObject> characterPanelInScene;
    private List<Image> characterHealthBar;
    private List<float> characterPanelOffset;

    private void Awake()
    {
        characterHolder = GameObject.Find("Character").transform;
        characterInScene = new List<Transform>();
        characterPanelInScene = new List<GameObject>();
        characterHealthBar = new List<Image>();
        characterPanelOffset = new List<float>();
}

    public void NewPanel(Transform character)
    {
        characterInScene.Add(character);
        characterPanelOffset.Add(character.GetComponent<Collider2D>().bounds.extents.y + 0.1f);
        GameObject panel = Instantiate(characterPanel_Instance, Camera.main.WorldToScreenPoint(character.position + Vector3.up * characterPanelOffset[characterPanelOffset.Count - 1]), Quaternion.identity, transform);
        characterHealthBar.Add(panel.transform.Find("Bar_Health").GetChild(0).GetComponent<Image>());
        characterPanelInScene.Add(panel);
        
    }

    private void FixedUpdate()
    {
        UpdatePanels();
    }

    private void UpdatePanels()
    {
        for (int i = 0; i < characterInScene.Count; i++)
        {
            if (characterInScene[i] == null)
            {
                int indexToRemove = i;
                characterInScene.RemoveAt(indexToRemove);
                characterHealthBar.RemoveAt(indexToRemove);
                GameObject panel = characterPanelInScene[indexToRemove];
                characterPanelInScene.RemoveAt(indexToRemove);
                Destroy(panel);
                break;
            }
            if (!characterInScene[i].GetComponent<Character>().isDead)
            {
                characterPanelInScene[i].transform.position = Camera.main.WorldToScreenPoint(characterInScene[i].position + Vector3.up * characterPanelOffset[i]);
                characterHealthBar[i].fillAmount = Mathf.Clamp((float)characterInScene[i].GetComponent<Character>().Health_Current / (float)characterInScene[i].GetComponent<Character>().Health_Base, 0, 1);
            }
            else
            {
                int indexToRemove = i;
                characterInScene.RemoveAt(indexToRemove);
                characterHealthBar.RemoveAt(indexToRemove);
                GameObject panel = characterPanelInScene[indexToRemove];
                characterPanelInScene.RemoveAt(indexToRemove);
                Destroy(panel);
                break;
            }
        }
    }
}
