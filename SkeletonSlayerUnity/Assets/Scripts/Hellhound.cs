using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hellhound : NPC
{
    private void Start()
    {
        StartCoroutine(Behavior());
    }

    IEnumerator Behavior()
    {
        yield return StartCoroutine(Patrol());
        Debug.Log("Target aquired! Target: "+target.name);
    }
}
