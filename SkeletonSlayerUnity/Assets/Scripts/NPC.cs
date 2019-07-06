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

    public IEnumerator Patrol()
    {
        MoveSpeed_Current = MoveSpeed_Base / 2;
        RaycastHit2D viewCheck;
        RaycastHit2D groundCheck;
        while (target == null)
        {
            if (!isStunned)
            {
                groundCheck = Physics2D.Raycast(transform.position + new Vector3(FacingDirection.x, FacingDirection.y, 0) * 0.25f, Vector2.down, 1f, GroundLayer);
                if (groundCheck)
                {
                    //Debug.Log("GroundCheck Passed: "+ groundCheck.collider.name);
                    viewCheck = Physics2D.Raycast(transform.position + new Vector3(FacingDirection.x, FacingDirection.y, 0) * 0.25f, FacingDirection, viewDistance, visible);
                    if (viewCheck)
                    {
                        //Debug.Log("Viewcheck Passed" + viewCheck.collider.name);
                        Move(Vector2.zero);
                        if (viewCheck.collider.tag == "Player")
                        {
                            target = viewCheck.collider.gameObject;
                        }
                        else
                        {
                            Turn();
                        }
                    }
                    else
                        Move(FacingDirection);
                }
                else
                {
                    //Debug.Log("No Ground detected");
                    Move(Vector2.zero);
                    Turn();
                }
            }
            yield return 100;
        }
        Debug.Log("Target found");
    }

    public IEnumerator Guard()
    {
        turnTimer = 3f;
        RaycastHit2D viewCheck;
        while (target == null)
        {
            if (!isStunned)
            {
                viewCheck = Physics2D.Raycast(transform.position + new Vector3(FacingDirection.x, FacingDirection.y, 0) * 0.25f, FacingDirection, viewDistance, visible);
                if (viewCheck)
                {
                    //Debug.Log("Viewcheck Passed" + viewCheck.collider.name);
                    if (viewCheck.collider.tag == "Player")
                    {
                        target = viewCheck.collider.gameObject;
                    }
                    else
                    {
                        if (turnTimer > 0)
                            turnTimer -= Time.deltaTime;
                        else
                        {
                            Turn();
                            turnTimer = 3f;
                        }
                    }
                }
                else
                {
                    if (turnTimer > 0)
                        turnTimer -= Time.deltaTime;
                    else
                    {
                        Turn();
                        turnTimer = 3f;
                    }
                }
            }
            yield return 100;
        }
        //Debug.Log("Target found");
    }
}
