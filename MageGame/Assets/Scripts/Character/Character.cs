using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IHealthManagement
{
    [SerializeField]
    private int health_base;
    private int health_current;
    public int Health_Base { get { return health_base; } set { health_base = value; } }
    [HideInInspector] public int Health_Current { get { return health_current; } set { health_current = value; } }
    public int DMG_Base;
    [HideInInspector] public int DMG_Current;
    public float MoveSpeed_Base;
    [HideInInspector] public float MoveSpeed_Current;
    public int JumpForce_Base;
    [HideInInspector] public int JumpForce_Current;
    public float dash_Speed;
    public float dash_Duration;
    public float dash_CoolDown;
    public float climbing_Speed;
    public GameObject[] spellInstances;
    [HideInInspector] public Spell[] spells;
    //public GameObject effect_Burn;
    //public GameObject effect_Freeze;
    //public GameObject effect_Root;
    //public GameObject effect_Wet;
    //public GameObject effect_Stun;
    public Transform projectileSpawn;
    public LayerMask GroundLayer;

    [HideInInspector] public float dash_cd;
    [HideInInspector] public float actionValue = 0;
    public bool isGrounded;
    public bool isWalking;
    public bool isJumping;
    public bool canClimb;
    public bool isClimbing;
    public bool isDashing;
    public bool isAttacking;
    public bool isFloating;
    public enum CastState { Default, Charge, Hold, Release }
    public CastState castingState = CastState.Default;
    public bool isDead;
    public bool isStunned;
    [HideInInspector] public bool climbLock;
    [HideInInspector] public Vector2 FacingDirection;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;

    private float tick;
    private int burn_Ticks;
    private int freeze_Ticks;
    private int root_Ticks;
    private int wet_Ticks;
    private int stun_Ticks;
    private Collider2D climbableObject;
    public enum AnimationState { Idle, Walk, Jump, Dash, Climb, CastCharge, CastHold, CastHoldWalk, CastRelease, AttackMelee, AttackRanged }
    private string currentAnimationState;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //effect_Burn = Instantiate(effect_Burn, transform);
        //effect_Burn.SetActive(false);
        //effect_Freeze = Instantiate(effect_Freeze, transform);
        //effect_Freeze.SetActive(false);
        //effect_Root = Instantiate(effect_Root, transform);
        //effect_Root.SetActive(false);
        //effect_Wet = Instantiate(effect_Wet, transform);
        //effect_Wet.SetActive(false);
        //effect_Stun = Instantiate(effect_Stun, transform);
        //effect_Stun.SetActive(false);
        //tick = 1f;
        Health_Current = Health_Base;
        DMG_Current = DMG_Base;
        MoveSpeed_Current = MoveSpeed_Base;
        JumpForce_Current = JumpForce_Base;
        FacingDirection = Vector2.right;
        if (spellInstances.Length > 0)
        {
            spells = new Spell[spellInstances.Length];
            for (int i = 0; i < spellInstances.Length; i++)
            {
                spellInstances[i] = Instantiate(spellInstances[i], transform);
                spells[i] = spellInstances[i].GetComponent<Spell>();
            }
        }
        if (GameObject.Find("GUI"))
            GameObject.Find("GUI").transform.Find("Panel_Character").GetComponent<CharacterPanelManager>().NewPanel(transform);
    }

    public virtual void Update()
    {
        if (isGrounded && !isDashing && !isAttacking)
        {
            if (isWalking)
            {
                if (castingState == CastState.Default)
                    ChangeAnimationState(AnimationState.Walk.ToString());
                else if (castingState == CastState.Charge)
                    ChangeAnimationState(AnimationState.CastHoldWalk.ToString());
                else if (castingState == CastState.Hold)
                    ChangeAnimationState(AnimationState.CastHoldWalk.ToString());
                else if (castingState == CastState.Release)
                    ChangeAnimationState(AnimationState.CastRelease.ToString());
            }
            else
            {
                if (castingState == CastState.Default)
                    ChangeAnimationState(AnimationState.Idle.ToString());
                else if (castingState == CastState.Charge)
                    ChangeAnimationState(AnimationState.CastCharge.ToString());
                else if (castingState == CastState.Hold)
                    ChangeAnimationState(AnimationState.CastHold.ToString());
                else if (castingState == CastState.Release)
                    ChangeAnimationState(AnimationState.CastRelease.ToString());
            }
        }
        if (isJumping && !isDashing && !isAttacking)
            ChangeAnimationState(AnimationState.Jump.ToString());
        if (isDashing)
            ChangeAnimationState(AnimationState.Dash.ToString());
        if (isClimbing)
        {
            anim.speed = Input.GetAxis("Vertical");
            ChangeAnimationState(AnimationState.Climb.ToString());
        }
        else if (anim.speed == 0)
            anim.speed = 1;
        if (isWalking && !isStunned)
        {
            anim.speed = Mathf.Clamp((float)MoveSpeed_Current / (float)MoveSpeed_Base, 0, 1);
        }
        if (dash_cd > 0)
            dash_cd -= Time.deltaTime;

        //anim.SetBool("IsGrounded", isGrounded);
        //anim.SetBool("IsWalking", isWalking);
        //anim.SetBool("IsClimbing", isClimbing);
        //anim.SetBool("IsDashing", isDashing);
        
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
                    SubtractHealth(1);
                    if (burn_Ticks == 0)
                        isStunned = false;
                }
                if (freeze_Ticks > 0)
                {
                    freeze_Ticks--;
                    if (freeze_Ticks == 0)
                        isStunned = false;
                }
                if (root_Ticks > 0)
                {
                    root_Ticks--;
                    if (root_Ticks == 0)
                        isStunned = false;
                }
                if (wet_Ticks > 0)
                {
                    wet_Ticks--;
                    if (wet_Ticks == 0)
                        isStunned = false;
                }
                if (stun_Ticks > 0)
                {
                    stun_Ticks--;
                    if (stun_Ticks == 0)
                        isStunned = false;
                }
                tick = .5f;
            }
        }
    }

    public void ChangeAnimationState (string newState)
    {
        if (currentAnimationState == newState)
            return;
        anim.Play(newState);
        currentAnimationState = newState;
    }

    public void AddHealth(int amount)
    {
        if (Health_Current + amount >= Health_Base)
            Health_Current = Health_Base;
        else
            Health_Current += amount;
    }

    public void SubtractHealth(int amount)
    {
        if (Health_Current - amount <= 0)
        {
            Health_Current = 0;
            Death();
        }
        else
            Health_Current -= amount;
    }

    public void Death()
    {
        isDead = true;
        Destroy(gameObject);
    }

    public void Walk(Vector2 direction, float speedModifier)
    {
        isWalking = true;
        float newVelX = direction.x * MoveSpeed_Current * speedModifier;
        if (Mathf.Abs(newVelX) > Mathf.Abs(rb.velocity.x) || newVelX < 0 && rb.velocity.x > 0 || newVelX > 0 && rb.velocity.x < 0)
            rb.velocity = new Vector2(newVelX, rb.velocity.y);
    }
    
    public void StopWalking()
    {
        isWalking = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void Turn(Vector2 direction)
    {
        FacingDirection = direction;
        transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void Jump(Vector2 direction, float forceMod)
    {
        if (isGrounded || isClimbing)
        {
            if (isClimbing)
            {
                isClimbing = false;
                rb.gravityScale = 1f;
                climbLock = true;
            }
            //anim.SetTrigger("Jump");
            isGrounded = false;
            isJumping = true;
            rb.AddForce((Vector2.up + direction) * JumpForce_Current * forceMod);
        }
    }

    public void Climb(Vector2 direction)
    {
        if (!isClimbing)
        {
            isClimbing = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            transform.position = new Vector2(climbableObject.transform.position.x, transform.position.y);
            rb.position = new Vector2(climbableObject.transform.position.x, transform.position.y);
        }
        if ((GetComponent<Collider2D>().bounds.center + (Vector3.up * GetComponent<Collider2D>().bounds.extents.y)).y < (climbableObject.bounds.center + (Vector3.up * climbableObject.bounds.extents.y)).y || direction.y < 0)
            rb.MovePosition(rb.position + direction * Time.fixedDeltaTime * climbing_Speed);
    }

    public void Dash(Vector2 direction)
    {
        if (!isDashing)
        {
            //anim.SetTrigger("Dash");
            isJumping = true;
            StartCoroutine(Dashing(direction));
        }
    }

    IEnumerator Dashing(Vector2 direction)
    {
        isDashing = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        float duration = dash_Duration;
        rb.velocity = direction * dash_Speed;
        while (duration > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rb.velocity.magnitude * Time.deltaTime, GroundLayer);
            if (hit)
                break;
            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rb.gravityScale = 1;
        dash_cd = dash_CoolDown;
        isDashing = false;
    }
    /*
    public void Teleport()
    {
        if (tele_cd > 0)
            return;
        tele_cd = teleport_Cooldown;
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
    */

    public void Knockback(Vector2 direction, float speed)
    {
        Stun(1);
        rb.velocity = direction * speed;
        isGrounded = false;
    }
    
    public void Float()
    {
        isFloating = true;
        rb.gravityScale = 0.4f;
    }

    public void Stun(int ticks)
    {
        //effect_Stun.SetActive(state);
        stun_Ticks = ticks;
        isStunned = true;
    }
    /*
    public void Burn(bool state)
    {
        //Debug.Log(name + " - Burn: " + state);
        effect_Burn.SetActive(state);
        if (state)
            burn_Ticks = 5;
        RpcBurn(state);
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
    */
    virtual public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Climbable")
        {
            climbableObject = collision.gameObject.GetComponent<Collider2D>();
            canClimb = true;
        }
        else if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            if (isJumping)
                isJumping = false;
            if (isFloating)
            {
                isFloating = false;
                rb.gravityScale = 1f;
            }
        }
    }

    virtual public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            if (isJumping)
                isJumping = false;
            if (isFloating)
            {
                isFloating = false;
                rb.gravityScale = 1f;
            }
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
        else if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
