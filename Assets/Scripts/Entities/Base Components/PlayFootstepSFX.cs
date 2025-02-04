using UnityEngine;

/** \brief
Attach this script to an object to play a footstep sound when the desired function is triggered.
Typically, the desired function is played by the animation itself using an event in the animation.
For example, Horus' walk animation has two frames which his foot touches the ground, at which an event
is placed to call PlayDefaultFootstep().

Documentation updated 1/27/2025
\author Stephen Nuttall
*/
public class PlayFootstepSFX : MonoBehaviour
{
    /// Reference to the ground detector, which is used to get the footstep sound should be played.
    [SerializeField] GroundDetector groundDetector;

    /// Plays a normal footstep sound, as opposed to a footstep sound for a jump or other impact.
    public void PlayDefaultFootstep()
    {
        if (groundDetector.isGrounded &&
        groundDetector.groundReference != null &&
        groundDetector.groundReference.TryGetComponent<TilemapFootstepSFX>(out var footstepSFX))
        {
            AudioManager.instance.PlayOneShot(footstepSFX.GetDefaultReference(), transform.position);
        }
    }
}
