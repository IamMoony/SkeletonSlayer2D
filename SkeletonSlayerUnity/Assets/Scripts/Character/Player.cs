using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : Character
{
    public Spell activeSpell;

    CinemachineVirtualCamera vcam;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        if (!hasAuthority)
            return;
        activeSpell = spells[0];
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
        if (Input.GetAxis("Horizontal") != 0)
        {
            Move(Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left, 1f);
        }
        else
        {
            Stop(true);
        }
        if (canClimb && !isClimbing)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                Climb(new Vector2(0, Input.GetAxis("Vertical")));
            }
        }
        if (canClimb && isClimbing)
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                Climb(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            Jump(Vector2.zero);
        }
        if (Input.GetButtonDown("Dash"))
        {
            Teleport();
        }
        if (Input.GetButtonDown("Shoot"))
        {
            if (isGrounded && !isWalking && activeSpell.cd <= 0)
            {
                Shoot(activeSpell.spellPrefab, FacingDirection);
            } 
        }
        if (Input.GetButtonDown("Activate"))
        {
            activeSpell.Activate(FacingDirection);
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            activeSpell = spells[0];
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            activeSpell = spells[1];
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            activeSpell = spells[2];
        }
    }
}
