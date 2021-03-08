using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character
{
    public float viewDistance;
    public LayerMask visible;

    public float attackRange_Melee;
    public float attackCooldown_Melee;
    public float attackRange_Ranged;
    public float attackCooldown_Ranged;
    public GameObject projectile_Ranged;
    public GameObject target;
    public RaycastHit2D[] objectsInView;
    public bool targetInView;
    public GameObject foreignProjectile;
    public bool projectileInView;
    public bool targetInRange_Melee;
    public float cooldown_Melee;
    public bool targetInRange_Ranged;
    public float cooldown_Ranged;
    public bool viewClear = true;
    public bool jumpClear = false;
    public bool floorClear = false;
    public bool pathClear = false;


    public enum Faction { Friendly, Neutral, Enemy };
    public Faction faction;

    public enum State { Normal, Alert };
    public State state;

    public enum Action { Guarding, Patrolling, Pursuing, Searching, Fleeing };
    public Action action;

    private float turnTimer;
    private Vector2 cSize;

    public override void Start()
    {
        base.Start();
        cSize = GetComponent<Collider2D>().bounds.size;
    }

    public override void Update()
    {
        base.Update();
        if (isDead)
        {
            Destroy(gameObject);
        }
        if (cooldown_Melee > 0)
            cooldown_Melee -= Time.deltaTime;
        if (cooldown_Ranged > 0)
            cooldown_Ranged -= Time.deltaTime;
    }

    public IEnumerator Routine_AttackMelee(bool jump)
    {
        //Debug.Log("Start Melee Attack Routine");
        if (isStunned)
            yield break;
        if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
            Turn(FacingDirection * -1);
        cooldown_Melee = attackCooldown_Melee;
        AttackMelee(jump);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
    }

    public void AttackMelee(bool jump)
    {
        isAttacking = true;
        ChangeAnimationState(AnimationState.AttackMelee.ToString());
        if (jump)
            rb.AddForce((Vector2.up + FacingDirection) * JumpForce_Current * 0.75f);
    }

    public IEnumerator Routine_AttackRanged()
    {
        //Debug.Log("Start Ranged Attack Routine");
        if (isStunned)
            yield break;
        if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
            Turn(FacingDirection * -1);
        cooldown_Ranged = attackCooldown_Ranged;
        AttackRanged();
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
    }

    public void AttackRanged()
    {
        isAttacking = true;
        ChangeAnimationState(AnimationState.AttackRanged.ToString());
        GameObject go = Instantiate(projectile_Ranged, projectileSpawn.position, Quaternion.Euler(FacingDirection == (Vector2)transform.right ? 0 : 180, 0, FacingDirection == (Vector2)transform.right ? 0 : 180));
        go.GetComponent<Projectile>().owner = this;
    }

    public IEnumerator GetInRange(bool melee)
    {
        Debug.Log("Start GetInRange Routine");
        while (melee ? !targetInRange_Melee : !targetInRange_Ranged)
        {
            if (isStunned)
                yield break;
            if (!IsTargetInSight())
            {
                //Target out of Sight
                target = null;
                yield break;
            }
            if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
                Turn(FacingDirection * -1);
            if (!IsTargetInRange(melee))
                Walk(FacingDirection, 1f);
            else
                StopWalking();
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator Patrol()
    {
        Debug.Log("Start Patrol Routine");
        while (target == null && foreignProjectile == null)
        {
            if (!isStunned && isGrounded)
            {
                floorClear = CheckFloor();
                if (!floorClear)
                {
                    Debug.Log("No floor detected - Stoping");
                    StopWalking();
                    yield return new WaitForSeconds(1f);
                    jumpClear = CheckJump();
                    if (!jumpClear)
                    {
                        Debug.Log("Jump not possible - Turning");
                        Turn(FacingDirection * -1);
                    }
                    else
                    {
                        Debug.Log("Jump TakeOff");
                        Jump(FacingDirection, 1f);
                        while (!isGrounded)
                            yield return new WaitForEndOfFrame();
                        Debug.Log("Jump Landing");
                    }
                }
                else
                {
                    Debug.Log("Floor detected");
                    pathClear = CheckPath();
                    if (!pathClear)
                    {
                        Debug.Log("Path obstructed - Turning");
                        Turn(FacingDirection * -1);
                    }
                    else
                    {
                        Debug.Log("Path clear");
                        viewClear = CheckView();
                        if (!viewClear)
                        {
                            Debug.Log("Something is in View");
                            IdentifyObjectsInView();
                            if (!targetInView)
                            {
                                Debug.Log("No target or projectile found - Moving");
                                Walk(FacingDirection, 0.5f);
                            }
                            else
                            {
                                Debug.Log("Target or projectile found - Stoping");
                                StopWalking();
                            }
                        }
                        else
                        {
                            Debug.Log("View clear - Moving");
                            Walk(FacingDirection, 0.5f);
                        }
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator Guard()
    {
        turnTimer = 3f;
        while (target == null)
        {
            if (!isStunned)
            {
                viewClear = CheckView();
                if (!viewClear)
                {
                    IdentifyObjectsInView();
                    if (targetInView)
                        break;
                }
                if (turnTimer > 0)
                    turnTimer -= Time.deltaTime;
                else
                {
                    Turn(FacingDirection * -1);
                    turnTimer = 3f;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void IdentifyObjectsInView()
    {
        for (int i = 0; i < objectsInView.Length; i++)
        {
            if (objectsInView[i].transform.tag == "Character")
            {
                //Debug.Log("Character in View");
                if (objectsInView[i].transform.GetComponent<Character>() is Player && !targetInView)
                {
                    //Debug.Log("its a player");
                    target = objectsInView[i].transform.gameObject;
                    targetInView = true;
                }
            }
            else if (objectsInView[i].transform.tag == "Projectile")
            {
                //Debug.Log("Projectile in View");
                if (objectsInView[i].transform.GetComponent<Projectile>().owner is Player && !projectileInView)
                {
                    //Debug.Log("its a player projectile");
                    foreignProjectile = objectsInView[i].transform.gameObject;
                    projectileInView = true;
                }
            }
        }
    }

    public bool CheckView()
    {
        //Debug.Log("Checking View");
        objectsInView = Physics2D.BoxCastAll(transform.position, cSize, 0, FacingDirection, viewDistance, visible);
        if (objectsInView.Length > 0)
            return true;
        return false;
    }

    public bool CheckFloor()
    {
        Debug.Log("Check Floor at Position: " + ((Vector2)transform.position + FacingDirection * cSize.x * 1.5f) + " with Size: " + cSize.x);
        if (Physics2D.OverlapCircle((Vector2)transform.position + FacingDirection * cSize.x * 1.5f, cSize.x * 2f, GroundLayer))
            return true;
        return false;
    }

    public bool CheckPath()
    {
        if (Physics2D.OverlapCircle((Vector2)transform.position + FacingDirection * cSize.x , 0.01f, GroundLayer))
            return false;
        return true;
    }

    public bool CheckJump()
    {
        if (Physics2D.OverlapCircle((Vector2)transform.position + FacingDirection * (JumpForce_Base * 0.01f), cSize.x, GroundLayer))
            return true;
        return false;
    }

    public bool IsTargetInSight()
    {
        RaycastHit2D check = Physics2D.Raycast(transform.position, target.transform.position - transform.position, viewDistance, visible);
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

    public bool IsTargetInRange(bool melee)
    {
        if (Vector2.Distance(target.transform.position, transform.position) <= (melee ? attackRange_Melee : attackRange_Ranged))
        {
            if (melee)
                targetInRange_Melee = true;
            targetInRange_Ranged = true;
            return true;
        }
        else
        {
            if (melee)
                targetInRange_Melee = false;
            else
                targetInRange_Ranged = false;
        }
        return false;
    }

    public bool ProjectileThreatCheck()
    {
        Vector2 nextProjectilePosition = (Vector2)foreignProjectile.transform.position + foreignProjectile.GetComponent<Projectile>().rb.velocity * Time.deltaTime;
        Vector2 nextCharacterPosition = (Vector2)transform.position + rb.velocity * Time.deltaTime;
        if (Vector2.Distance(nextProjectilePosition, nextCharacterPosition) > 1f)
            return false;
        return true;
    }
}
