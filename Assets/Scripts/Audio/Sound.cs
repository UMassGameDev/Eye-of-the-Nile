/**************************************************
Stores the data and properties of a given sound

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.5f;
}
