/**************************************************
Responsible for storing and playing the game's music and sound effects.
To trigger a sound effect from a script, use "AudioManager.Instance.PlaySFX(SOUND_EFFECT_NAME);"

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;  // To make the object persistent, it needs a reference to itself.
    public Sound[] musicSounds, sfxSounds;  // List of all music sound objects, List of all sfx sound objects.
    public AudioSource musicSource, sfxSource; // Object that plays a sound under its sound object’s settings. Loops vs does not loop the sound.

    // Starts playing the default music.
    void Start()
    {
        PlayMusic("default_theme");
    }

    // Makes this object persistent.
    // If this is the only AudioManager in the scene, don’t destroy it on reload. If there’s another AudioManager in the scene, destroy it.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Plays a sound object using musicSource.
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("ERROR PLAYING MUSIC");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.volume = s.volume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // Plays a sound object using sfxSource.
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("ERROR PLAYING SFX");
        }
        else
        {
            sfxSource.volume = s.volume;
            sfxSource.PlayOneShot(s.clip);
        }
    }

}
