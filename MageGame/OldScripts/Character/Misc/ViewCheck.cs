using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCheck : MonoBehaviour
{
    NPC Parent;

    void Awake()
    {
        Parent = transform.parent.GetComponent<NPC>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Parent.viewClear = false;
        Parent.objectsInView.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Parent.objectsInView.Remove(collision.gameObject);
        if (Parent.objectsInView.Count == 0)
            Parent.viewClear = true;
    }
}
