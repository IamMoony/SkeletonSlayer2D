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

    public LayerMask GroundLayer;

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

    public virtual void Update()
    {
        ANIM.SetBool("IsGrounded", GroundCheck());
    }

    public void Move(Vector2 direction)
    {
        ANIM.SetBool("IsWalking", direction != Vector2.zero);
        if (direction.x < 0 && transform.localScale.x > 0 || direction.x > 0 && transform.localScale.x < 0)
            Turn();
        RB.velocity = new Vector2(direction.x * MoveSpeed_Current, RB.velocity.y);
    }

    public bool GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GetComponent<Renderer>().bounds.size.y * 0.52f, GroundLayer);
        if (hit)
        {
            //Debug.Log("GroundCheck - Success");
            return true;
        }
        else
        {
            //Debug.Log("GroundCheck - Failed");
            //if (hit)
            //    Debug.Log("Raycasthit Name:" + hit.collider.name);
            return false;
        }
    }

    public void Turn()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void Jump()
    {
        if (GroundCheck())
        {
            ANIM.SetTrigger("Jump");
            RB.AddForce(Vector2.up * JumpForce_Current);
        }
    }
}
