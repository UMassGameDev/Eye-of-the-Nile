/**************************************************
Stores the data and properties of a given sound file.

Documentation updated 8/14/2024
**************************************************/
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;  // Name of this sound. This is how the sound will be referenced in scripts.
    public AudioClip clip;  // Reference to the audio file.

    [Range(0f, 1f)]
    public float volume = 0.5f;  // Volume of the sound. Range limited to 0 to 1.
}
