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

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        CmdSpawnPlayer();
    }

    [Command]
    void CmdSpawnPlayer()
    {
        player = Instantiate(playerPrefab);
        //gameManager.CmdAddPlayer(player);
        NetworkServer.Spawn(player, connectionToClient);
    }
}
