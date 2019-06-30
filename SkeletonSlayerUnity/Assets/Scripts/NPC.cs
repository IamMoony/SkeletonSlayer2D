using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character
{
    public float viewDistance;

    public GameObject target;

    public enum attitude { Friendly, Neutral, Enemy };
    public attitude Attitude;

    public enum state {Normal, Alert}
    public state State;

    public enum action {Guarding, Patrolling, Attacking, Searching, Fleeing};
    public action Action;

    public IEnumerator Patrol()
    {
        Move(transform.right);
        RaycastHit2D viewCheck = Physics2D.Raycast(transform.position + transform.right * 0.5f, transform.right, viewDistance);
        RaycastHit2D groundCheck = Physics2D.Raycast(transform.position + transform.right * 0.5f, Vector2.down, 10f);
        while (target == null)
        {
            if (groundCheck)
            {
                if (viewCheck)
                {
                    Move(Vector2.zero);
                    if (viewCheck.collider.tag == "Player")
                    {
                        target = viewCheck.collider.gameObject;
                    }
                    else
                    {
                        Turn();
                    }
                }
            }
            else
            {
                Turn();
            }
            yield return 10;
        }
    }
}
