using UnityEngine;

public class PlayFootstepSFX : MonoBehaviour
{
    [SerializeField] GroundDetector groundDetector;

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
