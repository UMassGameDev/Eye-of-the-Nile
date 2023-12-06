using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public LayerMask groundLayer;
    public bool isGrounded {get; private set;} = false;

    void FixedUpdate()
    {
        isGrounded = false;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
            isGrounded = true;
    }
}
