using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpellEffect : MonoBehaviour
{
    [HideInInspector] public Character owner;
    [HideInInspector] public AudioSource audioSource;
    public GameObject effect_Cast;
    public GameObject effect_Destroy;

    public virtual void Awake()
    {
        if (transform.Find("AudioSource"))
            audioSource = transform.Find("AudioSource").GetComponent<AudioSource>();
    }

    public void DestroyEffect()
    {
        if (transform.Find("AudioSource"))
        {
            Transform audio = transform.Find("AudioSource");
            audio.SetParent(null);
            Destroy(audio.gameObject, 5f);
        }
        Destroy(Instantiate(effect_Destroy, transform.position, Quaternion.identity), 5f);
        NetworkServer.Destroy(gameObject);
    }
}
