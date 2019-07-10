using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character
{
    public float viewDistance;
    public LayerMask visible;

    public float attackRange;
    public float attackDuration;

    public GameObject target;
    public bool inAttackRange;

    public enum faction { Friendly, Neutral, Enemy };
    public faction Faction;

    public enum attitude { Defensive, Passive, Aggressive };
    public attitude Attitude;

    public enum state {Normal, Alert};
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

    public bool CheckPath()
    {
        return !Physics2D.Raycast(transform.position, FacingDirection, .5f, GroundLayer);
    }

    public GameObject CheckView()
    {
        RaycastHit2D viewCheck = Physics2D.Raycast(transform.position, FacingDirection, viewDistance, visible);
        if (viewCheck)
            return viewCheck.collider.gameObject;
        else
            return null;
    }

    public bool CheckTarget()
    {
        RaycastHit2D check = Physics2D.Raycast(transform.position, target.transform.position - transform.position, 10f, visible);
        Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red, 1f);
        if (check)
        {
            //Debug.Log("TargetCheck Hit");
            if (check.collider.tag == "Player")
            {
                //Debug.Log("Player");
                return true;
            }
            else if (Vector2.Distance(transform.position, target.transform.position) < check.distance)
                return true;
        }
        else if (Vector2.Distance(transform.position, target.transform.position) < 1f)
            return true;
        return false;
    }

    public bool CheckJump()
    {
        return Physics2D.Raycast(transform.position + new Vector3(FacingDirection.x, FacingDirection.y, 0) * 2f + Vector3.up, Vector2.down, 3f, GroundLayer);
    }

    public IEnumerator Attack_Melee()
    {
        //Debug.Log("Start Attack Routine");
        while (inAttackRange)
        {
            ANIM.SetTrigger("Attack_Melee");
            yield return new WaitForSeconds(attackDuration);
            if (Vector2.Distance(target.transform.position, transform.position) > attackRange)
                inAttackRange = false;
        }
        //Debug.Log("Out of Range");
    }

    public IEnumerator Pursue()
    {
        //Debug.Log("Start Pursue Routine");
        while (!inAttackRange)
        {
            if (!CheckTarget())
            {
                target = null;
                yield break;
            }
            if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
                Turn();
            if (Vector2.Distance(target.transform.position, transform.position) < attackRange)
            {
                //Debug.Log("In Range");
                Move(Vector2.zero);
                inAttackRange = true;
            }
            else
            {
                Move(FacingDirection);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator Patrol()
    {
        //Debug.Log("Start Patrol Routine");
        MoveSpeed_Current = MoveSpeed_Base / 2;
        while (target == null)
        {
            if (!isStunned && isGrounded)
            {
                if (CheckFloor())
                {
                    if (!CheckPath())
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
                            if (view.tag == "Player")
                            {
                                Move(Vector2.zero);
                                target = view;
                                //Debug.Log("Target aquired: " + target.name);
                            }
                            else
                                Move(FacingDirection);
                        }
                    }
                }
                else
                {
                    Move(Vector2.zero);
                    yield return new WaitForSeconds(Random.Range(0.25f, 1.5f));
                    if (CheckJump())
                    {
                        Jump(FacingDirection);
                        isGrounded = false;
                        while (!isGrounded)
                            yield return new WaitForEndOfFrame();
                    }
                    else
                    {
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
                    if (view.tag == "Player")
                    {
                        Move(Vector2.zero);
                        target = view;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        yield return true;
    }
}
