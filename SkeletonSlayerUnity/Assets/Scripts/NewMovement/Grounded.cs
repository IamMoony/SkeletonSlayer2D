using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : MonoBehaviour
{
    GameObject MainPlayer;
    // Start is called before the first frame update
    void Start()
    {
        MainPlayer = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground")
        {
            MainPlayer.GetComponent<Move>().isGrounded = true;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground")
        {
            MainPlayer.GetComponent<Move>().isGrounded = false;
        }
    }
}
