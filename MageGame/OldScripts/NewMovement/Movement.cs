using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool isGrounded = false;
    // public bool isRunning = false;

    Animator animator;
    Rigidbody2D rigidBody2D;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        // Init
        animator = GetComponent<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Jump();
        // Movement horizontal
        /*
        Vector3 horizontalMovement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
        transform.position += horizontalMovement * Time.deltaTime * moveSpeed;
        */

        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            rigidBody2D.velocity = new Vector2(2, rigidBody2D.velocity.y);
            if(isGrounded)
            {
                animator.Play("Run");
            }
            spriteRenderer.flipX = false;

        } else if(Input.GetKey("a") || Input.GetKey("left"))
        {
            rigidBody2D.velocity = new Vector2(-2, rigidBody2D.velocity.x);
            animator.Play("Run");
            spriteRenderer.flipX = true;
        } else
        {
            animator.Play("Idle");
        }
       
    }
    // Jump
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
        }
    }
}


