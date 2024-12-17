using UnityEngine;
using System;
using System.Collections;

/*!
\brief Responsible for storing and playing the game's music and sound effects.
To trigger a sound effect from a script, use "AudioManager.Instance.PlaySFX(SOUND_EFFECT_NAME);"

Documentation updated 10/8/2024
\author Nick Bottari, Stephen Nuttall, Alexander Art
\todo Make pausing/unpausing the game pause/unpause all game sounds.
\todo Stop all sounds when exiting to main menu and play main menu theme.
*/
public class AudioManager : MonoBehaviour
{
    /// To make the object persistent, it needs a reference to itself.
    public static AudioManager Instance;  

    /// List of all music sound objects.
    public Sound[] musicSounds;
    /// List of all sfx sound objects.
    public Sound[] sfxSounds;  

    /// \brief Reference to an object that plays a sound under a given sound object’s settings (Loops vs does not loop the sound).
    /// This audio source is responsible for playing music, especially the non-looping beginning of a looping song.
    public AudioSource musicSourcePrimary;
    /// \brief Reference to an object that plays a sound under a given sound object’s settings (Loops vs does not loop the sound).
    /// This audio source is responsible for playing music, especially a looping song with a non-looping first part already using musicSourcePrimary.
    public AudioSource musicSourceSecondary;
    /// \brief Reference to an object that plays a sound under a given sound object’s settings (Loops vs does not loop the sound).
    /// This audio source is responsible for playing sound effects.
    public AudioSource sfxSource;

    // Reference to the DataManager, needed for accessing the volume settings.
    DataManager dataManager;

    /// <summary>
    /// Starts playing the default music.
    /// </summary>
    void Start()
    {
        PlayMusic("desertTheme_start", "desertTheme_loop");
    }

    /// <summary>
    /// Makes this object persistent.
    /// If this is the only AudioManager in the scene, don’t destroy it on reload. If there’s another AudioManager in the scene, destroy it.
    /// Also set reference to dataManager.
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

        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }
    
    /// <summary>
    /// Plays a sound object using musicSource.
    /// </summary>
    /// <param name="name">Name of the of the sound object to play</param>
    public void PlayMusic(string name)
    {
        // stop any music playing on the secondary music source
        musicSourceSecondary.Stop();
        
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound object '" + name + "' not found in Audio Manager");
        }
        else
        {
            musicSourcePrimary.clip = s.clip;
            musicSourcePrimary.volume = s.volume * dataManager.GetMasterVolumeSetting() * dataManager.GetMusicVolumeSetting();
            musicSourcePrimary.loop = true;
            musicSourcePrimary.Play();
        }
    }

    /// <summary>
    /// Plays a sound object using musicSource.
    /// In this override, a sound will be played to start the music, and another looped version wil continue to play afterwards.
    /// This is useful for music with a beginning section that we only want played once, allowing for a seemless loop.
    /// </summary>
    /// <param name="startName">Name of the sound object to play first, but only once.</param>
    /// <param name="loopName">Name of the sound object to play after the start one ends, looping forever.</param>
    public void PlayMusic(string startName, string loopName)
    {
        // stop any music playing on the secondary music source
        musicSourceSecondary.Stop();
        
        Sound startSound = Array.Find(musicSounds, x => x.name == startName);
        Sound loopSound = Array.Find(musicSounds, x => x.name == loopName);

        if (startSound == null)
        {
            Debug.LogWarning("Sound object '" + startSound + "' not found in Audio Manager");
        }
        else if (loopSound == null)
        {
            Debug.LogWarning("Sound object '" + loopSound + "' not found in Audio Manager");
        }
        else
        {
            musicSourcePrimary.clip = startSound.clip;
            musicSourcePrimary.volume = startSound.volume * dataManager.GetMasterVolumeSetting() * dataManager.GetMusicVolumeSetting();
            musicSourcePrimary.loop = false;
            musicSourcePrimary.Play();

            musicSourceSecondary.clip = loopSound.clip;
            musicSourceSecondary.volume = loopSound.volume * dataManager.GetMasterVolumeSetting() * dataManager.GetMusicVolumeSetting();
            musicSourceSecondary.loop = true;
            musicSourceSecondary.PlayDelayed(startSound.clip.length);
        }
    }

    /// <summary>
    /// Plays a sound object using sfxSource.
    /// </summary>
    /// <param name="name">Name of the of the sound object to play</param>
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound object '" + name + "' not found in Audio Manager");
        }
        else
        {
            sfxSource.volume = s.volume * dataManager.GetMasterVolumeSetting() * dataManager.GetSfxVolumeSetting();
            sfxSource.PlayOneShot(s.clip);
        }
    }

    /// <summary>
    /// Update the volume the music plays at.
    /// </summary>
    public void VolumeChanged()
    {
        // Find the sounds that the musicSourcePrimary and musicSourceSecondary are playing
        Sound primarySound = Array.Find(musicSounds, x => x.name == musicSourcePrimary.clip.name);
        Sound secondarySound = Array.Find(musicSounds, x => x.name == musicSourcePrimary.clip.name);
        // Update the volume of the musicSourcePrimary and musicSourceSecondary
        musicSourcePrimary.volume = primarySound.volume * dataManager.GetMasterVolumeSetting() * dataManager.GetMusicVolumeSetting();
        musicSourceSecondary.volume = primarySound.volume * dataManager.GetMasterVolumeSetting() * dataManager.GetMusicVolumeSetting();
    }
}
