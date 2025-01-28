using FMODUnity;
using UnityEngine;

/** \brief
Holds the references to the footsteps sounds this tilemap makes. For example, if an area is made of sand,
you can put this script on the tilemap collider (must be a tilemap that handles collision) and have entities
(with a PlayFootstepSFX script) make sand footstep sounds when walking.

Documentation updated 1/27/2025
\author Stephen Nuttall
*/
public class TilemapFootstepSFX : MonoBehaviour
{
    /// Reference to the footstep sound that will play when the entity is walking on the ground.
    [SerializeField] EventReference defaultFootstep;
    /// Reference to the footstep sound that will play when the entity lands from a jump.
    [SerializeField] EventReference jumpFootstep;

    /// Returns the reference to the footstep sound that will play when the entity is walking on the ground.
    public EventReference GetDefaultReference() { return defaultFootstep; }
    /// Returns the reference to the footstep sound that will play when the entity lands from a jump.
    public EventReference GetJumpReference() { return jumpFootstep; }
}
