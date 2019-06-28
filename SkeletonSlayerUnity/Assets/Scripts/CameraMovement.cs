using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;
    public float maxOffset;

    private void Update()
    {
        if (Vector2.Distance(transform.position, target.position) > maxOffset)
        {
            Vector2 newPos = Vector2.Lerp(transform.position, target.position, Time.deltaTime * moveSpeed);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
    }
}
