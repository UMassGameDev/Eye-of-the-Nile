/**************************************************
Responsible for storing and playing the game's music and sound effects.
To trigger a sound effect from a script, use "AudioManager.Instance.PlaySFX(SOUND_EFFECT_NAME);"

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

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
            musicSource.Play();
        }
    }

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
