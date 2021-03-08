using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public float holdDuration;
    private float hDur;

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
            Walk(direction, 1f);
        }
        else
            StopWalking();
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
            Vector2 cursorPos = GetCursorWorldPosition2D();
            Vector2 direction = cursorPos - (Vector2)transform.position;
            Dash(direction.normalized);
        }
        if (Input.GetButtonDown("Shoot"))
        {
            TryCastingSpell(0, GetCursorWorldPosition2D());
        }
        if (Input.GetButtonUp("Activate"))
        {
            if (hDur != 10)
                spells[0].Activate(GetCursorWorldPosition2D(), false);
            hDur = 0;
        }
        if (Input.GetButton("Activate"))
        {
            if (hDur < holdDuration)
                hDur += Time.deltaTime;
            else if (hDur != 10)
            {
                spells[0].Activate(GetCursorWorldPosition2D(), true);
                hDur = 10;
            }
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

    public Vector2 GetCursorWorldPosition2D()
    {
        Vector2 cPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        return cPos;
    }
}
