using FMODUnity;
using UnityEngine;

public class TilemapFootstepSFX : MonoBehaviour
{
    [SerializeField] EventReference defaultFootstep;
    [SerializeField] EventReference jumpFootstep;

    public EventReference GetDefaultReference() { return defaultFootstep; }
    public EventReference GetJumpReference() { return jumpFootstep; }
}
