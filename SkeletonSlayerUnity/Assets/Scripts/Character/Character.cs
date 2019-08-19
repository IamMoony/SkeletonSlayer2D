using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int HP_Base;
    [HideInInspector] public int HP_Current;
    public int DMG_Base;
    [HideInInspector] public int DMG_Current;
    public int MoveSpeed_Base;
    [HideInInspector] public int MoveSpeed_Current;
    public int JumpForce_Base;
    [HideInInspector] public int JumpForce_Current;
    public float dash_Speed;
    public float dash_Distance;
    public float teleport_Distance;
    public AnimationClip animation_PreShoot;
    public GameObject effect_Burn;
    public GameObject effect_Freeze;
    public GameObject effect_Root;
    public GameObject effect_Wet;
    public GameObject effect_Stun;

    public float actionValue = 0;
    public bool isGrounded;
    public bool isWalking;
    public bool canClimb;
    public bool isClimbing;
    public LayerMask GroundLayer;
    public bool isDashing;
    public Vector2 FacingDirection;
    public bool isDead;
    public bool isStunned;

    public Rigidbody2D rb;
    public Animator anim;
    private float tick;
    private int burn_Ticks;
    private int freeze_Ticks;
    private int root_Ticks;
    private int wet_Ticks;
    private int stun_Ticks;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        effect_Burn = Instantiate(effect_Burn, transform);
        effect_Burn.SetActive(false);
        effect_Freeze = Instantiate(effect_Freeze, transform);
        effect_Freeze.SetActive(false);
        effect_Root = Instantiate(effect_Root, transform);
        effect_Root.SetActive(false);
        effect_Wet = Instantiate(effect_Wet, transform);
        effect_Wet.SetActive(false);
        //effect_Stun = Instantiate(effect_Stun, transform);
        //effect_Stun.SetActive(false);
        tick = 1f;
        HP_Current = HP_Base;
        DMG_Current = DMG_Base;
        MoveSpeed_Current = MoveSpeed_Base;
        JumpForce_Current = JumpForce_Base;
        FacingDirection = Vector2.right;
    }

    public virtual void FixedUpdate()
    {
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsWalking", isWalking);
        anim.SetBool("IsClimbing", isClimbing);
        if (burn_Ticks > 0 || freeze_Ticks > 0 || root_Ticks > 0 || wet_Ticks > 0 || stun_Ticks > 0)
        {
            if (tick > 0)
            {
                tick -= Time.fixedDeltaTime;
            }
            else
            {
                if (burn_Ticks > 0)
                {
                    burn_Ticks--;
                    Damage(1);
                    if (burn_Ticks == 0)
                        Burn(false);
                }
                if (freeze_Ticks > 0)
                {
                    freeze_Ticks--;
                    if (freeze_Ticks == 0)
                        Freeze(false);
                }
                if (root_Ticks > 0)
                {
                    root_Ticks--;
                    if (root_Ticks == 0)
                        Root(false);
                }
                if (wet_Ticks > 0)
                {
                    wet_Ticks--;
                    if (wet_Ticks == 0)
                        Wet(false);
                }
                if (stun_Ticks > 0)
                {
                    stun_Ticks--;
                    if (stun_Ticks == 0)
                        Stun(false, 0);
                }
                tick = 1f;
            }
        }
        if (isClimbing && Input.GetAxis("Vertical") == 0)
        {
            anim.speed = 0;
        }
        else if (isWalking && !isStunned)
        {
            anim.speed = Mathf.Clamp((float)MoveSpeed_Current / (float)MoveSpeed_Base, 0, 1);
        }
        else if (anim.speed == 0 && !isStunned)
            anim.speed = 1;
    }

    public void Move(Vector2 direction, float speedMod)
    {
        isWalking = true;
        if (direction.x < 0 && FacingDirection == Vector2.right || direction.x > 0 && FacingDirection == Vector2.left)
            Turn();
        rb.velocity = new Vector2((direction.x * MoveSpeed_Current) * speedMod, rb.velocity.y);
    }

    public void Turn()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        FacingDirection *= -1f;
    }

    public void Jump(Vector2 direction)
    {
        if (isGrounded || isClimbing)
        {
            anim.SetTrigger("Jump");
            rb.AddForce((Vector2.up + direction) * JumpForce_Current);
        }
    }

    public void Dash()
    {
        if (!isDashing)
        {
            anim.SetTrigger("Dash");
            StartCoroutine(Dashing());
        }
    }

    IEnumerator Dashing()
    {
        isDashing = true;
        rb.gravityScale = 0;
        Vector2 targetPos = (Vector2)transform.position + FacingDirection * dash_Distance;
        Vector2 direction = FacingDirection;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.25f, GroundLayer);
        while (Vector2.Distance(transform.position, targetPos) > 0.1f && !hit)
        {
            rb.velocity = direction * dash_Speed;
            hit = Physics2D.Raycast(transform.position, direction, 0.25f, GroundLayer);
            yield return new WaitForEndOfFrame();
        }
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;
        isDashing = false;
    }

    public void Teleport()
    {
        anim.SetTrigger("Dash");
        StartCoroutine(Teleporting());
    }

    IEnumerator Teleporting()
    {
        Vector2 targetPos = (Vector2)transform.position + FacingDirection * teleport_Distance;
        Collider2D obstructed = Physics2D.OverlapCircle(targetPos, 0.1f, GroundLayer);
        if (obstructed)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, FacingDirection, teleport_Distance, GroundLayer);
            targetPos = hit.point;
        }
        transform.position = targetPos;
        yield return null;
    }

    public GameObject Shoot(GameObject projectile, Vector2 direction)
    {
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y) + direction * 0.25f;
        GameObject proj = Instantiate(projectile, spawnPos, Quaternion.Euler(direction == (Vector2)transform.right ? 0 : 180, 0, direction == (Vector2)transform.right ? 0 : 180));
        return proj;
    }

    public void Damage(int amount)
    {
        HP_Current = Mathf.Clamp(HP_Current - amount, 0, HP_Base);
        if (HP_Current == 0)
        {
            isDead = true;
        }
    }

    public void Knockback(Vector2 direction, int force)
    {
        rb.AddForce(direction * force);
    }

    public void Stun(bool state, int time)
    {
        //effect_Stun.SetActive(state);
        isStunned = state;
        if (state)
        {
            stun_Ticks = time;
            anim.speed = 0;
        }
        else
            anim.speed = 1;
    }

    public void Burn(bool state)
    {
        effect_Burn.SetActive(state);
        if (state)
            burn_Ticks = 5;
    }

    public void Freeze(bool state)
    {
        effect_Freeze.SetActive(state);
        MoveSpeed_Current = state ? 0 : MoveSpeed_Base;
        Wet(false);
        if (state)
        {
            freeze_Ticks = 5;
            Stun(state, 5);
            rb.velocity = Vector2.zero;
        }
    }

    public void Root(bool state)
    {
        effect_Root.SetActive(state);
        MoveSpeed_Current = state ? 0 : MoveSpeed_Base;
        if (state)
        {
            root_Ticks = 10;
            rb.velocity = Vector2.zero;
        }
    }

    public void Wet(bool state)
    {
        effect_Wet.SetActive(state);
        MoveSpeed_Current = state ? MoveSpeed_Base / 2 : MoveSpeed_Base;
        if (state)
            wet_Ticks = 10;
    }

    virtual public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Climbable")
        {
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Climbable")
        {
            canClimb = false;
            if (isClimbing)
            {
                isClimbing = false;
                rb.gravityScale = 1f;
            }
        }
    }

    public IEnumerator Cast(Spell spell)
    {
        anim.SetTrigger("Cast");
        while(actionValue < 1)
        {
            if (actionValue >= (spell.castTime - (animation_PreShoot == null ? 0 : animation_PreShoot.averageDuration)) / spell.castTime)
                anim.SetTrigger("Shoot");
            if (isGrounded && !isWalking)
                actionValue = Mathf.Clamp(actionValue + Time.deltaTime / spell.castTime, 0, 1);
            else
                break;
            yield return new WaitForEndOfFrame();
        }
        actionValue = 0;
        if (isGrounded && !isWalking)
        {
            spell.Cast((Vector2)transform.position + FacingDirection, this);
        }
    }

    public void Climb(Vector2 direction)
    {
        if (!isClimbing)
        {
            isClimbing = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        transform.position = Vector2.MoveTowards(transform.position, transform.position + new Vector3(0, direction.y, 0), Time.deltaTime * 2f);
    }
}
