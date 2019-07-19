using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCheck : MonoBehaviour
{
    NPC Parent;

    void Awake()
    {
        Parent = transform.parent.GetComponent<NPC>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.jumpClear = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Parent.jumpClear = false;
    }
}
