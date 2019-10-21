using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public Spell activeSpell;

    private void Start()
    {
        activeSpell = spells[0];
    }

    public override void Update()
    {
        base.Update();
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
        if (canClimb)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                Climb(new Vector2(0, Input.GetAxis("Vertical")));
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
                StartCoroutine(Cast(activeSpell));
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
