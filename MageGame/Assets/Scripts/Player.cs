using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public override void Update()
    {
        base.Update();
        if (isStunned || isDead || isDashing)
            return;
        if (Input.GetAxis("Horizontal") != 0 && !isClimbing)
        {
            Vector2 direction = Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left;
            if (direction != FacingDirection)
                Turn(direction);
            Move(direction, 1f);
        }
        if (canClimb && !climbLock)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                Vector2 direction = Input.GetAxis("Vertical") > 0 ? Vector2.up : Vector2.down;
                Climb(direction);
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            Jump(Vector2.zero, 1f);
        }
        if (Input.GetButtonDown("Dash") && dash_cd <= 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
            Vector2 direction = mousePosition - transform.position;
            Dash(direction.normalized);
        }
        if (Input.GetButtonDown("Shoot"))
        {
            //CastSpell
        }
        if (Input.GetButtonDown("Activate"))
        {
            //ActivateSpell(FacingDirection);
        }
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (activeSpellID.Length > 0)
                CmdCast(activeSpellID[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (activeSpellID.Length > 1)
                CmdCast(activeSpellID[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (activeSpellID.Length > 2)
                CmdCast(activeSpellID[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (activeSpellID.Length > 3)
                CmdCast(activeSpellID[3]);
        }
        */
    }
}
