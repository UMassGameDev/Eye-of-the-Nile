using UnityEngine;

/** \brief
This script forces the object it's attached to be in a specific location at the start of the scene no matter what.
This is a band-aid solution to stop the out of bounds problem in the Anubis scene, where the player would sometimes spawn in the wrong spot.

Documentation updated 2/3/2025
\author Stephen Nuttall
*/
public class ForceSpawnLocation : MonoBehaviour
{
    [SerializeField] Vector2 spawnLocation;

    void Start()
    {
        transform.position = spawnLocation;
    }
}
