using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;

public class Player : Character
{
    CinemachineVirtualCamera vcam;
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        activeSpellID = 0;
        vcam = GameObject.Find("Camera_Control").GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = transform;
    }

    public override void Update()
    {
        base.Update();
        if (!hasAuthority)
            return;
        if (isStunned)
            return;
        if (Input.GetAxis("Horizontal") != 0 && !isClimbing || Input.GetAxis("Horizontal") != 0 && isGrounded && isClimbing)
        {
            Vector2 direction = Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left;
            if (direction != FacingDirection)
                CmdTurn(direction);
            CmdMove(direction, 1f);
        }
        else
        {
            CmdStop(true);
        }
        if (canClimb && !climbLock)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                CmdClimb(new Vector2(0, Input.GetAxis("Vertical")));
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            CmdJump(Vector2.zero);
        }
        if (Input.GetButtonDown("Dash"))
        {
            CmdTeleport();
        }
        if (Input.GetButtonDown("Shoot"))
        {
            if (isGrounded && !isWalking && spells[activeSpellID].cd <= 0)
            {
                CmdCast();
            } 
        }
        if (Input.GetButtonDown("Activate"))
        {
            CmdActivateSpell(FacingDirection);
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            CmdChangeSpell(0);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            CmdChangeSpell(1);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            CmdChangeSpell(2);
        }
    }
}
