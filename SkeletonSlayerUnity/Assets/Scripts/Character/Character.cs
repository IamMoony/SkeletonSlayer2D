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
    public GameObject effect_Burn;
    public GameObject effect_Freeze;
    public GameObject effect_Root;
    public GameObject effect_Wet;
    public GameObject effect_Stun;

    public float actionValue = 0;
    public bool isGrounded;
    public bool isWalking;
    public bool isCasting;
    public bool isEvoking;
    public bool canClimb;
    public bool isClimbing;
    public LayerMask GroundLayer;
    public bool isDashing;
    public Vector2 FacingDirection;
    public bool isDead;
    public bool isStunned;

    private Rigidbody2D RB;
    public Animator ANIM;
    private float tick;
    private int burn_Ticks;
    private int freeze_Ticks;
    private int root_Ticks;
    private int wet_Ticks;
    private int stun_Ticks;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        ANIM = GetComponent<Animator>();
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
        ANIM.SetBool("IsGrounded", isGrounded);
        ANIM.SetBool("IsWalking", isWalking);
        ANIM.SetBool("IsCasting", isCasting);
        ANIM.SetBool("IsClimbing", isClimbing);
        if (burn_Ticks > 0 || freeze_Ticks > 0 || root_Ticks > 0)
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
                    Damage(1, Vector2.up);
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
            ANIM.speed = 0;
        }
        else if (isWalking)
        {
            ANIM.speed = Mathf.Clamp((float)MoveSpeed_Current / (float)MoveSpeed_Base, 0, 1);
        }
        else if (ANIM.speed == 0)
            ANIM.speed = 1;
    }

    public void Move(Vector2 direction)
    {
        isWalking = true;
        if (direction.x < 0 && FacingDirection == Vector2.right || direction.x > 0 && FacingDirection == Vector2.left)
            Turn();
        RB.velocity = new Vector2(direction.x * MoveSpeed_Current, RB.velocity.y);
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
            ANIM.SetTrigger("Jump");
            RB.AddForce((Vector2.up + direction) * JumpForce_Current);
        }
    }

    public void Dash()
    {
        if (!isDashing)
        {
            ANIM.SetTrigger("Dash");
            StartCoroutine(Dashing());
        }
    }

    IEnumerator Dashing()
    {
        isDashing = true;
        RB.gravityScale = 0;
        Vector2 targetPos = (Vector2)transform.position + FacingDirection * dash_Distance;
        Vector2 direction = FacingDirection;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.25f, GroundLayer);
        while (Vector2.Distance(transform.position, targetPos) > 0.1f && !hit)
        {
            RB.velocity = direction * dash_Speed;
            hit = Physics2D.Raycast(transform.position, direction, 0.25f, GroundLayer);
            yield return new WaitForEndOfFrame();
        }
        RB.velocity = Vector2.zero;
        RB.gravityScale = 1;
        isDashing = false;
    }

    public void Teleport()
    {
        ANIM.SetTrigger("Dash");
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
        ANIM.SetTrigger("Shoot");
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y) + direction * 0.25f;
        GameObject proj = Instantiate(projectile, spawnPos, Quaternion.Euler(0, 0, direction == (Vector2)transform.right ? 0 : 180));
        return proj;
    }

    public void Damage(int amount, Vector2 direction)
    {
        if (isGrounded)
            RB.velocity = direction + Vector2.up;
        HP_Current = Mathf.Clamp(HP_Current - amount, 0, HP_Base);
        if (HP_Current == 0)
        {
            isDead = true;
        }
    }

    public void Stun(bool state, int time)
    {
        //effect_Stun.SetActive(state);
        isStunned = state;
        if (state)
            stun_Ticks = time;
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
        //Stun(state, 5);
        Wet(false);
        if (state)
        {
            freeze_Ticks = 5;
            RB.velocity = Vector2.zero;
        }
    }

    public void Root(bool state)
    {
        effect_Root.SetActive(state);
        MoveSpeed_Current = state ? 0 : MoveSpeed_Base;
        if (state)
        {
            root_Ticks = 10;
            RB.velocity = Vector2.zero;
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
                RB.gravityScale = 1f;
            }
        }
    }

    public IEnumerator Cast(string spell, float castTime)
    {
        isCasting = true;
        while(actionValue < castTime)
        {
            if (isGrounded && !isWalking)
                actionValue = Mathf.Clamp(actionValue + Time.deltaTime, 0, 1);
            else
                break;
            yield return new WaitForEndOfFrame();
        }
        actionValue = 0;
        isCasting = false;
        if (isGrounded && !isWalking)
        {
            isEvoking = true;
            yield return StartCoroutine(spell);
            isEvoking = false;
        }
    }

    public void Climb(Vector2 direction)
    {
        if (!isClimbing)
        {
            isClimbing = true;
            RB.velocity = Vector2.zero;
            RB.gravityScale = 0;
        }
        transform.position = Vector2.MoveTowards(transform.position, transform.position + new Vector3(0, direction.y, 0), Time.deltaTime * 2f);
    }
}
