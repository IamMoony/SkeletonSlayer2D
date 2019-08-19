using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public GameObject spell_Firebolt;
    public GameObject spell_Waterbolt;
    public GameObject spell_Earthbolt;

    public Spell activeSpell;

    private void Start()
    {
        activeSpell = spell_Firebolt.GetComponent<Spell>();
    }

    public void Update()
    {
        if (isStunned)
            return;
        if (Input.GetAxis("Horizontal") != 0)
        {
            Move(Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left, 1f);
        }
        else
        {
            isWalking = false;
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
            activeSpell = spell_Firebolt.GetComponent<Spell>();
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            activeSpell = spell_Waterbolt.GetComponent<Spell>();
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            activeSpell = spell_Earthbolt.GetComponent<Spell>();
        }
    }
}
