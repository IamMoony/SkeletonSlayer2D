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
        if (!hasAuthority)
            return;
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
        if (Input.GetAxis("Horizontal") != 0 && !isClimbing || isGrounded && isClimbing)
        {
            CmdMove(Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left, 1f);
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
            NetworkClient.connection.identity.gameObject.GetComponent<PlayerConnection>().CmdActivateSpell(FacingDirection, gameObject);
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            NetworkClient.connection.identity.gameObject.GetComponent<PlayerConnection>().CmdChangeSpell(0, gameObject);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            NetworkClient.connection.identity.gameObject.GetComponent<PlayerConnection>().CmdChangeSpell(1, gameObject);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            NetworkClient.connection.identity.gameObject.GetComponent<PlayerConnection>().CmdChangeSpell(2, gameObject);
        }
    }
}
