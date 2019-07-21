using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public enum Element {Fire, Water, Earth, Air};
    public Element activeElement;
    public GameObject projectile_Fire;
    public GameObject projectile_Water;
    public GameObject projectile_Earth;
    public GameObject projectile_Air;

    private GameObject[] projectile_Instance;

    private void Start()
    {
        projectile_Instance = new GameObject[4];
        activeElement = Element.Fire;
    }

    public void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            Move(Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left);
        }
        else
        {
            isWalking = false;
        }
        if (canClimb)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                Climb(new Vector2(0, Input.GetAxis("Vertical")));
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            Jump(Vector2.zero);
        }
        if (Input.GetButtonDown("Dash"))
        {
            Teleport();
        }
        if (Input.GetButtonDown("Shoot"))
        {
            if (isGrounded && !isWalking)
            {
                StartCoroutine(Cast("Spell_Primary", 1f));
            } 
        }
        if (Input.GetButtonDown("Activate"))
        {
            if (projectile_Instance[(int)activeElement] != null)
            {
                if (!projectile_Instance[(int)activeElement].GetComponent<Projectile>().isActivated)
                    projectile_Instance[(int)activeElement].GetComponent<Projectile>().Activation(FacingDirection);
            }
        }
        if (Input.GetKey(KeyCode.Alpha1))
        {
            if (activeElement != Element.Fire)
            {
                activeElement = Element.Fire;
            }
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            if (activeElement != Element.Water)
            {
                activeElement = Element.Water;
            }
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            if (activeElement != Element.Earth)
            {
                activeElement = Element.Earth;
            }
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            if (activeElement != Element.Air)
            {
                activeElement = Element.Air;
            }
        }
    }

    public IEnumerator Spell_Primary()
    {
        if (activeElement == Element.Fire)
        {
            projectile_Instance[(int)Element.Fire] = Shoot(projectile_Fire, FacingDirection);
        }
        else if (activeElement == Element.Water)
        {
            projectile_Instance[(int)Element.Water] = Shoot(projectile_Water, FacingDirection);
        }
        else if (activeElement == Element.Earth)
        {
            projectile_Instance[(int)Element.Earth] = Shoot(projectile_Earth, FacingDirection);
        }
        else if (activeElement == Element.Air)
        {
            projectile_Instance[(int)Element.Air] = Shoot(projectile_Air, FacingDirection);
        }
        yield return new WaitForSeconds(0.5f);
    }

}
