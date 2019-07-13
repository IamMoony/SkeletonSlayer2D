using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCheck : MonoBehaviour
{
    NPC Parent;

    void Awake()
    {
        Parent = transform.parent.GetComponent<NPC>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.floorClear = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Parent.floorClear = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Parent.floorClear = false;
    }
}
