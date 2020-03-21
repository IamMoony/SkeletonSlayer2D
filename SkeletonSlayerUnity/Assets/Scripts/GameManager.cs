using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public List<GameObject> players;
    public List<GameObject> enemies;

    SpellSelection spellSelection;

    private void Awake()
    {
        spellSelection = GameObject.Find("Panel_SpellSelection").GetComponent<SpellSelection>();
        players = new List<GameObject>();
        enemies = new List<GameObject>();
    }

    [Command]
    public void CmdAddPlayer (GameObject player)
    {
        players.Add(player);
        if (players.Count == 1)
            spellSelection.localPlayer = player.GetComponent<Player>();
        RpcAddPlayer(player);
    }

    [ClientRpc]
    public void RpcAddPlayer(GameObject player)
    {
        if (!isClientOnly)
            return;
        players.Add(player);
        if (player.GetComponent<Player>().isLocalPlayer)
            spellSelection.localPlayer = player.GetComponent<Player>();
    }

    [Command]
    public void CmdAddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
        Time.timeScale = 0;
        spellSelection.StartSelection();
        RpcAddEnemy(enemy);
    }

    [ClientRpc]
    public void RpcAddEnemy(GameObject enemy)
    {
        if (!isClientOnly)
            return;
        enemies.Add(enemy);
        spellSelection.StartSelection();
    }

    [Command]
    public void CmdRemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        if (enemies.Count == 0)
            CmdEndBattle();
        RpcRemoveEnemy(enemy);
    }

    [ClientRpc]
    public void RpcRemoveEnemy(GameObject enemy)
    {
        if (!isClientOnly)
            return;
        enemies.Remove(enemy);
    }

    [Command]
    public void CmdStartBattle()
    {
        int rdyPlayers = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].GetComponent<Player>().inSelection)
                rdyPlayers++;
        }
        if (rdyPlayers == players.Count)
        {
            Time.timeScale = 1;
            RpcStartBattle();
        }
    }

    [ClientRpc]
    public void RpcStartBattle()
    {
        Time.timeScale = 1;
    }

    [Command]
    void CmdEndBattle()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].GetComponent<Player>().CmdChangeSpell(new int[0]);
        }
    }
}
