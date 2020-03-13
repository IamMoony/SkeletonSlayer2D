using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject playerPrefab;

    public GameObject playerInstance;

    private void Start()
    {
        if (!isLocalPlayer)
            return;
        CmdSpawnPlayer();
    }

    [Command]
    void CmdSpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab);
        NetworkServer.SpawnWithClientAuthority(playerInstance, connectionToClient);
    }
}
