using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public Transform transform;
    public float moveSpeed = 0.02f;
    public float jumpHeight = 0.5f;

    void Update()
    {
        if (Input.GetKey("a"))
        {
            transform.position -= new Vector3(moveSpeed,0.0f,0.0f);
        }
        if (Input.GetKey("d"))
        {
            transform.position += new Vector3(moveSpeed,0.0f,0.0f);
        }
        if (Input.GetKey("w") || Input.GetKey("space"))
        {
            // this is a bad system for jumping that should be replaced with something better
            transform.position += new Vector3(0.0f,jumpHeight,0.0f);
        }
    }
}
