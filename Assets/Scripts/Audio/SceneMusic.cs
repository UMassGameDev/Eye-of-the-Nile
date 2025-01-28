using FMODUnity;
using UnityEngine;

/** \brief
Plays music when the scene is loaded using the AudioManager.

Documentation updated 1/27/2025
\author Stephen Nuttall
*/
public class SceneMusic : MonoBehaviour
{
    [SerializeField] EventReference sceneMusic;

    void Start()
    {
        AudioManager.instance.PlayMusic(sceneMusic);
    }
}
