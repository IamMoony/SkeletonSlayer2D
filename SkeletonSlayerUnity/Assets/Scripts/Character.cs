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
    public float actionValue = 0;
    
    public bool isGrounded;
    public bool isWalking;
    public bool isCasting;
    public bool canClimb;
    public bool isClimbing;
    public LayerMask GroundLayer;
    public bool isDashing;
    public Vector2 FacingDirection;
    public bool isDead;
    public bool isStunned;

    private Rigidbody2D RB;
    public Animator ANIM;
    private GameObject Effect_Burn;
    private GameObject Effect_Freeze;
    private GameObject Effect_Root;
    private float tick;
    private int Burn_Ticks;
    private int Freeze_Ticks;
    private int Root_Ticks;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        ANIM = GetComponent<Animator>();
        Effect_Burn = transform.Find("Effect_Burn").gameObject;
        Effect_Burn.SetActive(false);
        Effect_Freeze = transform.Find("Effect_Freeze").gameObject;
        Effect_Freeze.SetActive(false);
        Effect_Root = transform.Find("Effect_Root").gameObject;
        Effect_Root.SetActive(false);
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
        if (Burn_Ticks > 0 || Freeze_Ticks > 0 || Root_Ticks > 0)
        {
            if (tick > 0)
            {
                tick -= Time.fixedDeltaTime;
            }
            else
            {
                if (Burn_Ticks > 0)
                {
                    Burn_Ticks--;
                    Damage(1);
                    if (Burn_Ticks == 0)
                        Burn(false);
                }
                if (Freeze_Ticks > 0)
                {
                    Freeze_Ticks--;
                    if (Freeze_Ticks == 0)
                        Freeze(false);
                }
                if (Root_Ticks > 0)
                {
                    Root_Ticks--;
                    if (Root_Ticks == 0)
                        Root(false);
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
        isWalking = direction != Vector2.zero;
        if (direction.x < 0 && FacingDirection == Vector2.right || direction.x > 0 && FacingDirection == Vector2.left)
            Turn();
        RB.velocity = new Vector2(direction.x * MoveSpeed_Current, RB.velocity.y);
    }

    public void Turn()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        FacingDirection = FacingDirection == Vector2.right ? Vector2.left : Vector2.right;
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
            isDashing = true;
            ANIM.SetTrigger("Dash");
            StartCoroutine(Dashing());
        }
    }

    IEnumerator Dashing()
    {
        Vector2 startPos = transform.position;
        Vector2 targetPos = startPos + FacingDirection * 2f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetPos - startPos, 0.25f, GroundLayer);
        while (Vector2.Distance(transform.position, targetPos) > 0.05f && !hit)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 25f * Time.deltaTime);
            hit = Physics2D.Raycast(transform.position, targetPos - startPos, 0.25f, GroundLayer);
            yield return 1;
        }
        isDashing = false;
    }

    public void Shoot(GameObject projectile, Vector2 direction)
    {
        ANIM.SetTrigger("Shoot");
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y) + direction * 0.25f;
        Destroy(Instantiate(projectile, spawnPos, Quaternion.Euler(0, 0, direction == Vector2.right ? 0 : 180)), 5f);
    }

    public void Lob(GameObject projectile, Vector2 direction)
    {
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y + 0.25f) + direction * 0.25f;
        Destroy(Instantiate(projectile, spawnPos, Quaternion.Euler(0, 0, direction == Vector2.right ? 45 : 135)), 5f);
    }

    public void GroundAttack(GameObject projectile, Vector2 direction)
    {
        Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y - 0.5f) + direction * 0.25f;
        Destroy(Instantiate(projectile, spawnPos, Quaternion.Euler(0, 0, direction == Vector2.right ? 0 : 180)), 5f);
    }

    public void Damage(int amount)
    {
        HP_Current = Mathf.Clamp(HP_Current - amount, 0, HP_Base);
        if (HP_Current == 0)
        {
            isDead = true;
        }
    }

    public void Burn(bool state)
    {
        Effect_Burn.SetActive(state);
        Burn_Ticks = 5;
    }

    public void Freeze(bool state)
    {
        Effect_Freeze.SetActive(state);
        MoveSpeed_Current = state ? 0 : MoveSpeed_Base;
        if (state)
        {
            Freeze_Ticks = 5;
            RB.velocity = Vector2.zero;
            isStunned = true;
        }
        else
            isStunned = false;
    }

    public void Root(bool state)
    {
        Effect_Root.SetActive(state);
        MoveSpeed_Current = state ? 0 : MoveSpeed_Base;
        if (state)
        {
            Root_Ticks = 10;
            RB.velocity = Vector2.zero;
        }
    }

    virtual public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Projectile.projectileElement element = collision.gameObject.GetComponent<Projectile>().element;
            int dmg = collision.gameObject.GetComponent<Projectile>().dmg;
            Damage(dmg);
            if (element == Projectile.projectileElement.Fire)
            {
                Burn(true);
            }
            else if (element == Projectile.projectileElement.Ice)
            {
                Freeze(true);
            }
            else if (element == Projectile.projectileElement.Earth)
            {
                Root(true);
            }
        }
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

    public IEnumerator Cast(GameObject projectile)
    {
        isCasting = true;
        while(actionValue < 1)
        {
            if (isGrounded && !isWalking)
                actionValue = Mathf.Clamp(actionValue + Time.deltaTime, 0, 1);
            else
                break;
            yield return 1;
        }
        isCasting = false;
        if (isGrounded && !isWalking)
            Shoot(projectile, FacingDirection);
        actionValue = 0;
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
