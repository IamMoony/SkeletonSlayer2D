using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        if (!isLocalPlayer)
            return;
        CmdSpawnPlayer();
    }

    [Command]
    void CmdSpawnPlayer()
    {
        GameObject go = Instantiate(playerPrefab);
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    [Command]
    public void CmdSpawnProjectile(GameObject projectile, Vector2 direction, Vector2 spawnPos, GameObject owner)
    {
        GameObject proj = Instantiate(projectile, spawnPos, Quaternion.Euler(direction == (Vector2)transform.right ? 0 : 180, 0, direction == (Vector2)transform.right ? 0 : 180));
        proj.GetComponent<Projectile>().owner = owner.GetComponent<Character>();
        NetworkServer.SpawnWithClientAuthority(proj, connectionToClient);
    }
}
