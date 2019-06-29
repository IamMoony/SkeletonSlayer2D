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

    public bool isGrounded;
    public LayerMask GroundLayer;
    public bool isDashing;

    private Rigidbody2D RB;
    private Animator ANIM;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        ANIM = GetComponent<Animator>();
        HP_Current = HP_Base;
        DMG_Current = DMG_Base;
        MoveSpeed_Current = MoveSpeed_Base;
        JumpForce_Current = JumpForce_Base;
    }

    void FixedUpdate()
    {
        ANIM.SetBool("IsGrounded", isGrounded);
    }

    public void Move(Vector2 direction)
    {
        ANIM.SetBool("IsWalking", direction != Vector2.zero);
        if (direction.x < 0 && transform.localScale.x > 0 || direction.x > 0 && transform.localScale.x < 0)
            Turn();
        RB.velocity = new Vector2(direction.x * MoveSpeed_Current, RB.velocity.y);
    }

    public void Turn()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            ANIM.SetTrigger("Jump");
            RB.AddForce(Vector2.up * JumpForce_Current);
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
        Vector2 targetPos = startPos + (transform.localScale.x > 0 ? Vector2.right : Vector2.left) * 2f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetPos - startPos, 0.25f, GroundLayer);
        while (Vector2.Distance(transform.position, targetPos) > 0.05f && !hit)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 25f * Time.deltaTime);
            hit = Physics2D.Raycast(transform.position, targetPos - startPos, 0.25f, GroundLayer);
            yield return 1;
        }
        isDashing = false;
    }
}
