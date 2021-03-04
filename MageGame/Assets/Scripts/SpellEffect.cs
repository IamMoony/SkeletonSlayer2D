using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    [HideInInspector] public Character owner;
    [HideInInspector] public AudioSource audioSource;
    public GameObject vfxDestroy;

    public virtual void Awake()
    {
        if (transform.Find("AudioSource"))
            audioSource = transform.Find("AudioSource").GetComponent<AudioSource>();
    }

    public void DestroySelf()
    {
        //if (transform.Find("AudioSource"))
        //{
        //    Transform audio = transform.Find("AudioSource");
        //    audio.SetParent(null);
        //    Destroy(audio.gameObject, 1f);
        //}
        Destroy(Instantiate(vfxDestroy, transform.position, Quaternion.identity), 5f);
        Destroy(gameObject);
    }
}
