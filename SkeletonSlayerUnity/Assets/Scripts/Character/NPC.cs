using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character
{
    public float viewDistance;
    public LayerMask visible;

    public float meleeAttackRange;
    public AnimationClip meleeAttackAnimation;
    public float rangedAttackRange;
    public AnimationClip rangedAttackAnimation;
    public GameObject rangedAttackProjectile;

    public List<GameObject> objectsInView;
    public GameObject target;
    public bool targetInRange;
    public bool viewClear = true;
    public bool jumpClear = true;
    public bool floorClear = true;
    public bool pathClear = true;
    

    public enum faction { Friendly, Neutral, Enemy };
    public faction Faction;

    public enum attitude { Defensive, Passive, Aggressive };
    public attitude Attitude;

    public enum state {Normal, Alert};
    public state State;

    public enum action {Guarding, Patrolling, Pursuing, Searching, Fleeing};
    public action Action;

    private float turnTimer;

    private void Start()
    {
        objectsInView = new List<GameObject>();
    }

    public override void Update()
    {
        base.Update();
        if (isDead)
        {
            Destroy(gameObject,0.1f);
        }
    }

    public IEnumerator Attack_Melee()
    {
        //Debug.Log("Start Melee Attack Routine");
        while (targetInRange)
        {
            if (isStunned)
                yield break;
            if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
                Turn();
            anim.SetTrigger("Attack_Melee");
            yield return new WaitForSeconds(meleeAttackAnimation.averageDuration);
            if (Vector2.Distance(target.transform.position, transform.position) > meleeAttackRange)
                targetInRange = false;
        }
        //Debug.Log("Out of Range");
    }

    public IEnumerator Attack_Ranged()
    {
        //Debug.Log("Start Ranged Attack Routine");
        while (targetInRange)
        {
            if (isStunned)
                yield break;
            if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
                Turn();
            anim.SetTrigger("Attack_Ranged");
            yield return new WaitForSeconds(rangedAttackAnimation.averageDuration);
            Shoot(rangedAttackProjectile, FacingDirection);
            if (Vector2.Distance(target.transform.position, transform.position) > rangedAttackRange)
                targetInRange = false;
        }
        //Debug.Log("Out of Range");
    }

    public IEnumerator Cast_Spell()
    {
        //Debug.Log("Start Ranged Attack Routine");
        while (targetInRange)
        {
            if (isStunned)
                yield break;
            if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
                Turn();
            yield return new WaitForSeconds(spells[0].cd);
            yield return StartCoroutine(Cast(spells[0]));
            if (Vector2.Distance(target.transform.position, transform.position) > rangedAttackRange)
                targetInRange = false;
        }
        //Debug.Log("Out of Range");
    }

    public IEnumerator Pursue()
    {
        //Debug.Log("Start Pursue Routine");
        while (!targetInRange)
        {
            if (isStunned)
                yield break;
            if (!TargetInSight())
            {
                target = null;
                yield break;
            }
            if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
                Turn();
            if (Vector2.Distance(target.transform.position, transform.position) < (rangedAttackRange > meleeAttackRange ? rangedAttackRange : meleeAttackRange))
            {
                //Debug.Log("In Range");
                isWalking = false;
                targetInRange = true;
            }
            else
            {
                Move(FacingDirection, 1f);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator Patrol()
    {
        //Debug.Log("Start Patrol Routine");
        while (target == null)
        {
            if (!isStunned && isGrounded)
            {
                if (!floorClear)
                {
                    //Debug.Log("No floor detected - Stoping");
                    isWalking = false;
                    yield return new WaitForSeconds(Random.Range(0.25f, 1.5f));
                    if (!jumpClear)
                    {
                        //Debug.Log("Jump not possible - Turning");
                        Turn();
                    }
                    else
                    {
                        //Debug.Log("Jump TakeOff");
                        Jump(FacingDirection);
                        isGrounded = false;
                        while (!isGrounded)
                            yield return new WaitForEndOfFrame();
                        //Debug.Log("Jump Landing");
                    }
                }
                else
                {
                    //Debug.Log("Floor detected");
                    if (!pathClear)
                    {
                        //Debug.Log("Path obstructed - Turning");
                        Turn();
                    }
                    else
                    {
                        //Debug.Log("Path clear");
                        if (!viewClear)
                        {
                            //Debug.Log("Something is in View");
                            if (!GetTarget())
                            {
                                //Debug.Log("No Target found - Moving");
                                Move(FacingDirection, 0.5f);
                            }
                            else
                            {
                                //Debug.Log("Target found - Stoping");
                                isWalking = false;
                            }
                        }
                        else
                        {
                            //Debug.Log("View clear - Moving");
                            Move(FacingDirection, 0.5f);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator Guard()
    {
        turnTimer = 3f;
        while (target == null)
        {
            if (!isStunned)
            {
                if (!viewClear)
                {
                    if (GetTarget())
                       break;
                }
                if (turnTimer > 0)
                    turnTimer -= Time.deltaTime;
                else
                {
                    Turn();
                    turnTimer = 3f;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public bool GetTarget()
    {
        for (int i = 0; i < objectsInView.Count; i++)
        {
            if (objectsInView[i].tag == "Character")
            {
                if (objectsInView[i].GetComponent<Character>() is Player)
                {
                    target = objectsInView[i];
                    return true;
                }
                else
                    objectsInView.RemoveAt(i);
            }
        }
        return false;
    }

    public bool TargetInSight()
    {
        RaycastHit2D check = Physics2D.Raycast(transform.position, target.transform.position - transform.position, 10f, visible);
        if (check)
        {
            if (check.collider.tag == "Character")
            {
                return true;
            }
            else if (Vector2.Distance(transform.position, target.transform.position) < check.distance)
                return true;
        }
        else if (Vector2.Distance(transform.position, target.transform.position) < 1f)
            return true;
        return false;
    }
}
