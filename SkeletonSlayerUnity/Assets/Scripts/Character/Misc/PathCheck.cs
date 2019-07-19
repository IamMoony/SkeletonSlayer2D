using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCheck : MonoBehaviour
{
    NPC Parent;

    void Awake()
    {
        Parent = transform.parent.GetComponent<NPC>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.pathClear = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Parent.pathClear = true;
    }
}
