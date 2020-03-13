using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NPC_Spawner : NetworkBehaviour
{
    public GameObject npcPrefab;
    public bool turn;
    private Transform characterHolder;
    private bool hasFired;

    private void Awake()
    {
        characterHolder = GameObject.Find("Enemies").transform;
    }

    private void OnBecameVisible()
    {
        if (!hasFired)
        {
            CmdSpawn();
            hasFired = true;
        }
    }

    [Command]
    private void CmdSpawn()
    {
        GameObject go = Instantiate(npcPrefab, transform.position, Quaternion.identity, characterHolder);
        NetworkServer.Spawn(go);
        if (turn)
            go.GetComponent<Character>().CmdTurn(go.GetComponent<Character>().FacingDirection * -1);
    }
}
