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
        else if (isGrounded)
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
            TryCastingSpell(0);
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

    private void TryCastingSpell(int spellID)
    {
        if (!spells[spellID].IsOnCooldown)
            StartCoroutine(CastSpell(spellID));
    }

    private void CastReleased()
    {
        castingState = CastState.Default;
    }

    private IEnumerator CastSpell(int spellID)
    {
        GameObject castVFX;
        if (spells[spellID].vfxSpellCast != null)
            castVFX = Instantiate(spells[spellID].vfxSpellCast, projectileSpawn.position, Quaternion.identity, transform);
        else
            castVFX = null;
        castingState = CastState.Charge;
        actionValue = 0;
        while (actionValue < 1)
        {
            //if (actionValue >= (spells[spellID].spellCastTime - (animation_PreShoot == null ? 0 : animation_PreShoot.averageDuration)) / spells[spellID].spellCastTime)
            //    anim.SetTrigger("Shoot");
            if (Input.GetButton("Shoot"))
                actionValue = Mathf.Clamp(actionValue + Time.deltaTime / spells[spellID].spellCastTime, 0, 1);
            else
                break;
            castVFX.transform.localScale = new Vector3(actionValue, actionValue, 1);
            yield return new WaitForEndOfFrame();
        }
        castingState = CastState.Hold;
        while (Input.GetButton("Shoot"))
            yield return new WaitForEndOfFrame();
        castingState = CastState.Release;
        Invoke("CastReleased", 0.3f);
        if (castVFX)
            Destroy(castVFX);
        Vector2 direction = projectileSpawn.transform.InverseTransformPoint(GetCursorWorldPosition2D());
        GameObject effect = Instantiate(spells[spellID].spellEffectPrefab[actionValue == 1 ? 1 : 0], projectileSpawn.position, Quaternion.Euler(new Vector3(0, FacingDirection == Vector2.right ? 0 : 180, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)));
        effect.GetComponent<SpellEffect>().owner = this;
        if (actionValue != 1)
            effect.GetComponent<Projectile>().targetPosition = GetCursorWorldPosition2D();
        spells[spellID].AddSpellEffectInstance(effect, true);
    }

    private Vector2 GetCursorWorldPosition2D()
    {
        Vector2 cPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        return cPos;
    }
}
