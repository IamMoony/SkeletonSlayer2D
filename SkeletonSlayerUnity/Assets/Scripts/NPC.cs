using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character
{
    public float viewDistance;
    public LayerMask visible;

    public GameObject target;

    public enum faction { Friendly, Neutral, Enemy };
    public faction Faction;

    public enum attitude { Defensive, Passive, Aggressive };
    public attitude Attitude;

    public enum state {Normal, Alert}
    public state State;

    public enum action {Guarding, Patrolling, Pursuing, Searching, Fleeing};
    public action Action;

    private float turnTimer;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isDead)
        {
            Destroy(gameObject,0.1f);
        }
    }

    public bool CheckFloor()
    {
        return Physics2D.Raycast(transform.position + new Vector3(FacingDirection.x, FacingDirection.y, 0) * 0.25f, Vector2.down, 1f, GroundLayer);
    }

    public bool CheckWall()
    {
        return Physics2D.Raycast(transform.position + new Vector3(FacingDirection.x, FacingDirection.y, 0) * 0.25f, FacingDirection, .25f, GroundLayer);
    }

    public GameObject CheckView()
    {
        RaycastHit2D viewCheck = Physics2D.Raycast(transform.position + new Vector3(FacingDirection.x, FacingDirection.y, 0) * 0.25f, FacingDirection, viewDistance, visible);
        if (viewCheck)
            return viewCheck.collider.gameObject;
        else
            return null;
    }

    public bool CheckJump()
    {
        return Physics2D.Raycast(transform.position + new Vector3(FacingDirection.x, FacingDirection.y, 0) * 2f + Vector3.up, Vector2.down, 3f, GroundLayer);
    }

    public IEnumerator Patrol()
    {
        MoveSpeed_Current = MoveSpeed_Base / 2;
        while (target == null)
        {
            if (!isStunned && isGrounded)
            {
                if (CheckFloor())
                {
                    if (CheckWall())
                    {
                        Turn();
                    }
                    else
                    {
                        GameObject view = CheckView();
                        if (view == null)
                        {
                            Move(FacingDirection);
                        }
                        else
                        {
                            Move(Vector2.zero);
                            target = view;
                        }
                    }
                }
                else
                {
                    Move(Vector2.zero);
                    yield return new WaitForSeconds(Random.Range(0.25f, 1.5f));
                    if (CheckJump())
                    {
                        Debug.Log("JumpCheck Passed!");
                        Jump(FacingDirection);
                        isGrounded = false;
                        while (!isGrounded)
                            yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        Debug.Log("JumpCheck Failed!");
                        Turn();
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        yield return true;
    }

    public IEnumerator Guard()
    {
        turnTimer = 3f;
        while (target == null)
        {
            if (!isStunned)
            {
                GameObject view = CheckView();
                if (view == null)
                {
                    if (turnTimer > 0)
                        turnTimer -= Time.deltaTime;
                    else
                    {
                        Turn();
                        turnTimer = 3f;
                    }
                }
                else
                {
                    target = view;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        yield return true;
    }
}
