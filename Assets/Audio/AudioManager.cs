using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

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
        Debug.Log("playing music" + s.name);

        if (s == null)
        {
            Debug.Log("ERROR PLAYING MUSIC");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        Debug.Log("playing sfx" + s.name);
        if (s == null)
        {
            Debug.Log("ERROR PLAYING SFX");
        }
        else if (!sfxSource.isPlaying)
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

}
