using FMODUnity;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] EventReference sceneMusic;

    void Start()
    {
        AudioManager.instance.PlayMusic(sceneMusic);
    }
}
