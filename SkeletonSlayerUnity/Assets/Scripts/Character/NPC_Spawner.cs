using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NPC_Spawner : NetworkBehaviour
{
    public GameObject npcPrefab;
    public GameObject npcInstance;
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
            for (int i = 0; i < NetworkServer.connections.Count; i++)
            {
                NetworkServer.connections[i].identity.GetComponent<PlayerConnection>().playerInstance.GetComponent<Player>().StartCombat(npcInstance.GetComponent<Character>());
            }
            hasFired = true;
        }
    }

    [Command]
    private void CmdSpawn()
    {
        npcInstance = Instantiate(npcPrefab, transform.position, Quaternion.identity, characterHolder);
        NetworkServer.Spawn(npcInstance);
        if (turn)
            npcInstance.GetComponent<Character>().CmdTurn(npcInstance.GetComponent<Character>().FacingDirection * -1);
    }
}
