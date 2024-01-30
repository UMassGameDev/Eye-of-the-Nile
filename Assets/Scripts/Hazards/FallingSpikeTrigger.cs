/**************************************************
Script for the falling spike hazard.
When a player enters the trigger zone beneath the falling spike, it will unfreeze the spike's gravity.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            transform.parent.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
        }
    }
}
