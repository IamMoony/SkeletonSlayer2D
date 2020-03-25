using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpellEffect : MonoBehaviour
{
    [HideInInspector] public Character owner;
    public GameObject effect_Cast;
    public GameObject effect_Destroy;

    public void DestroyEffect()
    {
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            Destroy(child.gameObject, 1f);
            child.SetParent(null);
        }
        Destroy(Instantiate(effect_Destroy, transform.position, Quaternion.identity), 5f);
        NetworkServer.Destroy(gameObject);
    }
}
