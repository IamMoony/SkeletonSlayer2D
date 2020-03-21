using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

public class Player : Character
{
    public List<int> activeSpellID;

    CinemachineVirtualCamera vcam;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        vcam = GameObject.Find("Camera_Control").GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = transform;
        GameObject.Find("Panel_SpellSelection").GetComponent<SpellSelection>().localPlayer = this;
        GameObject.Find("Panel_SpellSelection").GetComponent<SpellSelection>().StartSelection();
    }

    public override void Update()
    {
        base.Update();
        if (!hasAuthority || isStunned || Time.timeScale == 0)
            return;
        if (Input.GetAxis("Horizontal") != 0 && !isClimbing && knockTime <= 0 || Input.GetAxis("Horizontal") != 0 && isGrounded && isClimbing)
        {
            Vector2 direction = Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left;
            if (direction != FacingDirection)
                CmdTurn(direction);
            CmdMove(direction, 1f);
        }
        else if (isGrounded && knockTime <= 0)
        {
            CmdStop(true);
        }
        if (canClimb && !climbLock)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                Vector2 direction = Input.GetAxis("Vertical") > 0 ? Vector2.up : Vector2.down;
                CmdClimb(direction);
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            CmdJump(Vector2.zero, 1f);
        }
        if (Input.GetButtonDown("Dash"))
        {
            CmdTeleport();
        }
        if (Input.GetButtonDown("Shoot"))
        {
            if (spellBook.primarySpell.cd <= 0)
            {
                CmdCast(-1);
            } 
        }
        if (Input.GetButtonDown("Activate"))
        {
            CmdActivateSpell(FacingDirection);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (activeSpellID.Count > 0)
            CmdCast(activeSpellID[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (activeSpellID.Count > 1)
                CmdCast(activeSpellID[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (activeSpellID.Count > 2)
                CmdCast(activeSpellID[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (activeSpellID.Count > 3)
                CmdCast(activeSpellID[3]);
        }
    }
}
