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
}
