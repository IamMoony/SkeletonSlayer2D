using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject playerPrefab;

    private GameObject player;

    private void Start()
    {
        if (!isLocalPlayer)
            return;
        CmdSpawnPlayer();
    }

    [Command]
    void CmdSpawnPlayer()
    {
        player = Instantiate(playerPrefab);
        NetworkServer.SpawnWithClientAuthority(player, connectionToClient);
    }

    [Command]
    public void CmdCast(Vector2 direction, Vector2 spawnPos, GameObject owner)
    {
        owner.GetComponent<Character>().spells[owner.GetComponent<Character>().activeSpellID].Cast(direction, spawnPos, owner.GetComponent<Character>());
        RpcCast(direction, spawnPos, owner);
        //NetworkServer.SpawnWithClientAuthority(owner.GetComponent<Character>().activeSpell.spellInstance, connectionToClient);
    }

    [Command]
    public void CmdActivateSpell(Vector2 direction, GameObject owner)
    {
        owner.GetComponent<Character>().spells[owner.GetComponent<Character>().activeSpellID].Activate(direction);
        RpcActivateSpell(direction, owner);
    }

    [Command]
    public void CmdChangeSpell(int id, GameObject owner)
    {
        owner.GetComponent<Character>().activeSpellID = id;
    }

    [ClientRpc]
    void RpcCast(Vector2 direction, Vector2 spawnPos, GameObject owner)
    {
        owner.GetComponent<Character>().spells[owner.GetComponent<Character>().activeSpellID].Cast(direction, spawnPos, owner.GetComponent<Character>());
    }

    [ClientRpc]
    void RpcActivateSpell(Vector2 direction, GameObject owner)
    {
        owner.GetComponent<Character>().spells[owner.GetComponent<Character>().activeSpellID].Activate(direction);
    }
}
