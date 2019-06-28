using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    void Start()
    {
        
    }

    public void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            Move(Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left);
        }
        else
        {
            Move(Vector2.zero);
        }
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
}
