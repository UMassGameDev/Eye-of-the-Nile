using UnityEngine;

[System.Serializable]
/**
\brief Stores the data and properties of a given sound file.

Documentation updated 1/24/2025
\author Nick Bottari, Stephen Nuttall
\deprecated This is script part of the original sound system that's no longer used.
\note This class does not inhert from monobehavior, so it does not have access to unity functions such as Start() or Update().
*/
[System.Obsolete("This is script part of the original sound system that's no longer used.")]
public class Sound
{
    /// \brief Name of this sound. This is how the sound will be referenced in scripts.
    public string name;
    /// \brief Reference to the audio file.
    public AudioClip clip;

    [Range(0f, 1f)]
    /// \brief Volume of the sound. Range limited to 0 to 1.
    public float volume = 0.5f;
}
