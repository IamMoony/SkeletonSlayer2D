﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NPC : Character
{
    public float viewDistance;
    public LayerMask visible;

    public float attackRange_Melee;
    public float attackCooldown_Melee;
    public AnimationClip attackAnimation_Melee;
    public float attackRange_Ranged;
    public float attackCooldown_Ranged;
    public AnimationClip attackAnimation_Ranged;

    public List<GameObject> objectsInView;
    public GameObject target;
    public bool targetInRange_Melee;
    public float cooldown_Melee;
    public bool targetInRange_Ranged;
    public float cooldown_Ranged;
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
        if (cooldown_Melee > 0)
            cooldown_Melee -= Time.deltaTime;
        if (cooldown_Ranged > 0)
            cooldown_Ranged -= Time.deltaTime;
    }

    public IEnumerator Attack_Melee()
    {
        //Debug.Log("Start Melee Attack Routine");
        if (isStunned || cooldown_Melee > 0)
            yield break;
        if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
            CmdTurn();
        cooldown_Melee = attackCooldown_Melee;
        CmdAttackMelee();
        yield return new WaitForSeconds(attackAnimation_Melee.averageDuration);
    }

    [Command]
    public void CmdAttackMelee()
    {
        anim.SetTrigger("Attack_Melee");
        RpcAttackMelee();
    }

    [ClientRpc]
    public void RpcAttackMelee()
    {
        if (!isClientOnly)
            return;
        anim.SetTrigger("Attack_Melee");
    }

    public IEnumerator Attack_Ranged()
    {
        //Debug.Log("Start Ranged Attack Routine");
        if (isStunned || cooldown_Ranged > 0)
            yield break;
        if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
            CmdTurn();
        cooldown_Ranged = attackCooldown_Ranged;
        CmdTriggerRangedAnimation();
        yield return new WaitForSeconds(attackAnimation_Ranged.averageDuration);
        CmdAttackRanged();
    }

    [Command]
    public void CmdTriggerRangedAnimation()
    {
        anim.SetTrigger("Attack_Ranged");
        RpcTriggerRangedAnimation();
    }

    [ClientRpc]
    public void RpcTriggerRangedAnimation()
    {
        if (!isClientOnly)
            return;
        anim.SetTrigger("Attack_Ranged");
    }

    [Command]
    public void CmdAttackRanged()
    {
        GameObject go = Instantiate(defaultProjectile, projectileSpawn.position, Quaternion.Euler(FacingDirection == (Vector2)transform.right ? 0 : 180, 0, FacingDirection == (Vector2)transform.right ? 0 : 180));
        go.GetComponent<Projectile>().owner = this;
        RpcAttackRanged();
    }

    [ClientRpc]
    public void RpcAttackRanged()
    {
        if (!isClientOnly)
            return;
        GameObject go = Instantiate(defaultProjectile, projectileSpawn.position, Quaternion.Euler(FacingDirection == (Vector2)transform.right ? 0 : 180, 0, FacingDirection == (Vector2)transform.right ? 0 : 180));
        go.GetComponent<Projectile>().owner = this;
    }

    public IEnumerator Cast_Spell()
    {
        //Debug.Log("Start Cast_Spell Routine");
        if (isStunned)
            yield break;
        if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
            CmdTurn();
        //yield return new WaitForSeconds(spells[0].cd);
        if (spells[0].cd > 0)
            yield break;
        CmdCast();
        yield return new WaitForSeconds(spells[activeSpellID].castTime);
    }

    public IEnumerator GetInRange(bool melee)
    {
        //Debug.Log("Start GetInRange Routine");
        while (melee ? !targetInRange_Melee : !targetInRange_Ranged)
        {
            if (isStunned)
                yield break;
            if (!TargetInSight())
            {
                //Target out of Sight
                target = null;
                yield break;
            }
            if (target.transform.position.x < transform.position.x && FacingDirection == Vector2.right || target.transform.position.x > transform.position.x && FacingDirection == Vector2.left)
                CmdTurn();
            if (!TargetInRange(melee))
                CmdMove(FacingDirection, 1f);
            else
                CmdStop(false);
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
                    CmdStop(false);
                    yield return new WaitForSeconds(Random.Range(0.25f, 1.5f));
                    if (!jumpClear)
                    {
                        //Debug.Log("Jump not possible - Turning");
                        CmdTurn();
                    }
                    else
                    {
                        //Debug.Log("Jump TakeOff");
                        CmdJump(FacingDirection);
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
                        CmdTurn();
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
                                CmdMove(FacingDirection, 0.5f);
                            }
                            else
                            {
                                //Debug.Log("Target found - Stoping");
                                CmdStop(false);
                            }
                        }
                        else
                        {
                            //Debug.Log("View clear - Moving");
                            CmdMove(FacingDirection, 0.5f);
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
                    CmdTurn();
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

    public bool TargetInRange(bool melee)
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
}
