using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour

  
{

    Animator animator;
    Rigidbody2D rb2D;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {

        if(Input.GetKey("d") || Input.GetKey("right")) {
            rb2D.velocity = new Vector2(2, 0);
        } else if(Input.GetKey("a") || Input.GetKey("left"))
        {
            rb2D.velocity = new Vector2(-2, 0);
        }

        if(Input.GetKey("space"))
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, 3);
        }
    }
  
            
    
       
    }
