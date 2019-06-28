using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    Character Parent;

    void Awake()
    {
        Parent = transform.parent.GetComponent<Character>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Parent.isGrounded = false;
    }
}
