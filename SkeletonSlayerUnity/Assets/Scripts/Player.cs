﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public GameObject projectile;

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
        if (Input.GetButtonDown("Dash"))
        {
            Dash();
        }
        if (Input.GetButtonDown("Shoot"))
        {
            if (isGrounded && !isWalking)
                StartCoroutine(Cast(projectile));
        }
    }
}
