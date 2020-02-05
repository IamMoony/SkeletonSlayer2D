using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Character : NetworkBehaviour
{
    public int HP_Base;
    [HideInInspector, SyncVar] public int HP_Current;
    public int DMG_Base;
    [HideInInspector] public int DMG_Current;
    public int MoveSpeed_Base;
    [HideInInspector] public int MoveSpeed_Current;
    public int JumpForce_Base;
    [HideInInspector] public int JumpForce_Current;
    public float dash_Speed;
    public float dash_Distance;
    public float teleport_Distance;
    public float climbing_Speed;
    public Spell[] spells;
    [HideInInspector, SyncVar] public int activeSpellID;
    public AnimationClip animation_PreShoot;
    public GameObject effect_Burn;
    public GameObject effect_Freeze;
    public GameObject effect_Root;
    public GameObject effect_Wet;
    public GameObject effect_Stun;
    public Transform projectileSpawn;
    public GameObject defaultProjectile;

    [HideInInspector] public float actionValue = 0;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isWalking;
    [HideInInspector] public bool canClimb;
    [HideInInspector] public bool isClimbing;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isStunned;
    [HideInInspector] public bool climbLock;
    [HideInInspector, SyncVar] public Vector2 FacingDirection;
    [HideInInspector] public LayerMask GroundLayer;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;

    private float tick;
    private int burn_Ticks;
    private int freeze_Ticks;
    private int root_Ticks;
    private int wet_Ticks;
    private int stun_Ticks;
    private Collider2D climbableObject;

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
        if (GameObject.Find("GUI"))
            GameObject.Find("GUI").transform.Find("Panel_Character").GetComponent<CharacterPanelManager>().NewPanel(transform);
    }

    public virtual void Update()
    {
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsWalking", isWalking);
        anim.SetBool("IsClimbing", isClimbing);
        if (burn_Ticks > 0 || freeze_Ticks > 0 || root_Ticks > 0 || wet_Ticks > 0 || stun_Ticks > 0)
        {
            if (tick > 0)
            {
                tick -= Time.deltaTime;
            }
            else
            {
                if (burn_Ticks > 0)
                {
                    burn_Ticks--;
                    CmdDamage(1);
                    if (burn_Ticks == 0)
                        CmdBurn(false);
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
        for (int i = 0; i < spells.Length; i++)
        {
            if (spells[i].cd > 0)
                spells[i].cd -= Time.deltaTime;
        }
    }

    [Command]
    public void CmdMove(Vector2 direction, float speedMod)
    {
        isWalking = true;
        rb.velocity = new Vector2((direction.x * MoveSpeed_Current) * speedMod, rb.velocity.y);
        RpcMove(direction, speedMod);
    }

    [ClientRpc]
    public void RpcMove(Vector2 direction, float speedMod)
    {
        if (!isClientOnly)
            return;
        isWalking = true;
        rb.velocity = new Vector2((direction.x * MoveSpeed_Current) * speedMod, rb.velocity.y);
    }

    [Command]
    public void CmdStop(bool snappy)
    {
        isWalking = false;
        if (snappy)
            rb.velocity = new Vector2(0, rb.velocity.y);
        RpcStop(snappy);
    }

    [ClientRpc]
    public void RpcStop(bool snappy)
    {
        if (!isClientOnly)
            return;
        isWalking = false;
        if (snappy)
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    [Command]
    public void CmdTurn()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        FacingDirection *= -1f;
        RpcTurn();
    }

    [ClientRpc]
    public void RpcTurn()
    {
        if (!isClientOnly)
            return;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        FacingDirection *= -1f;
    }

    [Command]
    public void CmdJump(Vector2 direction)
    {
        if (isGrounded || isClimbing)
        {
            if (isClimbing)
            {
                isClimbing = false;
                rb.gravityScale = 1f;
                climbLock = true;
            }
            anim.SetTrigger("Jump");
            rb.AddForce((Vector2.up + direction) * JumpForce_Current);
            RpcJump(direction);
        }
    }

    [ClientRpc]
    public void RpcJump(Vector2 direction)
    {
        if (!isClientOnly)
            return;
        if (isGrounded || isClimbing)
        {
            if (isClimbing)
            {
                isClimbing = false;
                rb.gravityScale = 1f;
                climbLock = true;
            }
            anim.SetTrigger("Jump");
            rb.AddForce((Vector2.up + direction) * JumpForce_Current);
        }
    }

    [Command]
    public void CmdClimb(Vector2 direction)
    {
        if (!isClimbing)
        {
            isClimbing = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            transform.position = new Vector2(climbableObject.transform.position.x, transform.position.y);
        }
        if ((GetComponent<Collider2D>().bounds.center + (Vector3.up * GetComponent<Collider2D>().bounds.extents.y)).y < (climbableObject.bounds.center + (Vector3.up * climbableObject.bounds.extents.y)).y || direction.y < 0)
            rb.MovePosition(Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, Time.deltaTime * climbing_Speed));
        RpcClimb(direction);
    }

    [ClientRpc]
    public void RpcClimb(Vector2 direction)
    {
        if (!isClientOnly)
            return;
        if (!isClimbing)
        {
            isClimbing = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            transform.position = new Vector2(climbableObject.transform.position.x, transform.position.y);
        }
        if ((GetComponent<Collider2D>().bounds.center + (Vector3.up * GetComponent<Collider2D>().bounds.extents.y)).y < (climbableObject.bounds.center + (Vector3.up * climbableObject.bounds.extents.y)).y || direction.y < 0)
            rb.MovePosition(Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, Time.deltaTime * climbing_Speed));
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
    [Command]
    public void CmdTeleport()
    {
        anim.SetTrigger("Dash");
        Vector2 targetPos = (Vector2)transform.position + FacingDirection * teleport_Distance;
        Collider2D obstructed = Physics2D.OverlapCircle(targetPos, 0.1f, GroundLayer);
        if (obstructed)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, FacingDirection, teleport_Distance, GroundLayer);
            targetPos = hit.point;
        }
        transform.position = targetPos;
        RpcTeleport();
    }

    [ClientRpc]
    public void RpcTeleport()
    {
        if (!isClientOnly)
            return;
        anim.SetTrigger("Dash");
        Vector2 targetPos = (Vector2)transform.position + FacingDirection * teleport_Distance;
        Collider2D obstructed = Physics2D.OverlapCircle(targetPos, 0.1f, GroundLayer);
        if (obstructed)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, FacingDirection, teleport_Distance, GroundLayer);
            targetPos = hit.point;
        }
        transform.position = targetPos;
    }

    [Command]
    public void CmdDamage(int amount)
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

    [Command]
    public void CmdBurn(bool state)
    {
        Debug.Log(name + " - Burn: " + state);
        effect_Burn.SetActive(state);
        if (state)
            burn_Ticks = 5;
        RpcBurn(state);
    }

    [ClientRpc]
    public void RpcBurn(bool state)
    {
        if (!isClientOnly)
            return;
        Debug.Log(name + " - Burn: " + state);
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
            climbableObject = collision.gameObject.GetComponent<Collider2D>();
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Climbable")
        {
            canClimb = false;
            climbLock = false;
            if (isClimbing)
            {
                isClimbing = false;
                rb.gravityScale = 1f;
            }
        }
    }

    [Command]
    public void CmdCast()
    {
        StartCoroutine(Cast());
        RpcCast();
    }

    [ClientRpc]
    public void RpcCast()
    {
        if (!isClientOnly)
            return;
        StartCoroutine(Cast());
    }

    public IEnumerator Cast()
    {
        anim.SetTrigger("Cast");
        while(actionValue < 1)
        {
            if (actionValue >= (spells[activeSpellID].castTime - (animation_PreShoot == null ? 0 : animation_PreShoot.averageDuration)) / spells[activeSpellID].castTime)
                anim.SetTrigger("Shoot");
            if (isGrounded && !isWalking)
                actionValue = Mathf.Clamp(actionValue + Time.deltaTime / spells[activeSpellID].castTime, 0, 1);
            else
                break;
            yield return new WaitForEndOfFrame();
        }
        actionValue = 0;
        if (isGrounded && !isWalking)
        {
            spells[activeSpellID].Cast(FacingDirection, projectileSpawn.position, this);
        }
    }
}
