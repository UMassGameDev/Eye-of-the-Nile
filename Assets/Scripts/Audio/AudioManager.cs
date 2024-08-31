using UnityEngine;
using System;

/*!
\brief Responsible for storing and playing the game's music and sound effects.
To trigger a sound effect from a script, use "AudioManager.Instance.PlaySFX(SOUND_EFFECT_NAME);"

Documentation updated 8/14/2024
\author Nick Bottari, Stephen Nuttall*/
public class AudioManager : MonoBehaviour
{
    /// \brief To make the object persistent, it needs a reference to itself.
    public static AudioManager Instance;  
    /// \brief List of all music sound objects, List of all sfx sound objects.
    public Sound[] musicSounds, sfxSounds;  
    /// \brief Object that plays a sound under its sound object’s settings. Loops vs does not loop the sound.
    public AudioSource musicSource, sfxSource; 

    /// <summary>
    /// Starts playing the default music.
    /// </summary>
    void Start()
    {
        PlayMusic("default_theme");
    }

    /// <summary>
    /// Makes this object persistent.
    /// If this is the only AudioManager in the scene, don’t destroy it on reload. If there’s another AudioManager in the scene, destroy it.
    /// </summary>
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
    
    /// <summary>
    /// Plays a sound object using musicSource.
    /// </summary>
    /// <param name="name"></param>
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

    /// <summary>
    /// Plays a sound object using sfxSource.
    /// </summary>
    /// <param name="name"></param>
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
