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
    public void CmdChangeSpell(int id, GameObject owner)
    {
        owner.GetComponent<Character>().activeSpellID = id;
    }

    [Command]
    public void CmdActivateSpell(Vector2 direction, GameObject owner)
    {
        owner.GetComponent<Character>().spells[owner.GetComponent<Character>().activeSpellID].Activate(direction);
        RpcActivateSpell(direction, owner);
    }

    [ClientRpc]
    void RpcActivateSpell(Vector2 direction, GameObject owner)
    {
        owner.GetComponent<Character>().spells[owner.GetComponent<Character>().activeSpellID].Activate(direction);
    }
}
