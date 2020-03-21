using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject playerPrefab;

    private GameObject player;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

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
        gameManager.CmdAddPlayer(player);
        NetworkServer.SpawnWithClientAuthority(player, connectionToClient);
    }
}
