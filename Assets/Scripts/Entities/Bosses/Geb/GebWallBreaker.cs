using UnityEngine;

/** \brief
Script for the charge attack hitbox on Geb to destroy his walls on collision.

Documentation updated 1/11/2025
\author Alexander Art
*/
public class GebWallBreaker : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        // If the collided object is one of Geb's walls.
        if (col.gameObject.name == "Geb Wall(Clone)")
        {
            col.gameObject.GetComponent<ProtectiveWall>().DestroyWall();
        }
    }
}
