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

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        HP_Current = HP_Base;
        DMG_Current = DMG_Base;
        MoveSpeed_Current = MoveSpeed_Base;
        JumpForce_Current = JumpForce_Base;
    }

    public void Move(Vector2 direction)
    {
        RB.velocity = new Vector2(direction.x * MoveSpeed_Current, RB.velocity.y);
    }

    public bool GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GetComponent<Renderer>().bounds.size.y * 0.52f, GroundLayer);
        if (hit)
        {
            Debug.Log("GroundCheck - Success");
            return true;
        }
        else
        {
            Debug.Log("GroundCheck - Failed");
            if (hit)
                Debug.Log("Raycasthit Name:" + hit.collider.name);
            return false;
        }
    }

    public void Jump()
    {
        if (GroundCheck())
        {
            RB.AddForce(Vector2.up * JumpForce_Current);
        }
    }
}
