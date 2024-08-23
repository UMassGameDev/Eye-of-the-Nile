using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/** \brief 
Put this script on any object you want to automatically despawn after a certain amount of time.

Documentation updated 8/23/2024

\author Stephen Nuttall
*/
public class DespawnTimer : MonoBehaviour
{
    /// Time until object despawns, in seconds.
    public float seconds = 10f;
    /// If true, the object will be destroyed.
    public bool destroyGameObject = true;
    /// Event that's triggered when the despawn timer runs out. A function can be subscribed to this in the Unity Editor.
    public UnityEvent onDespawn;

    /// Wait 10 seconds, then trigger the event and destroy the object (if enabled).
    IEnumerator timer()
    {
        yield return new WaitForSeconds(seconds);
        onDespawn?.Invoke();
        if (destroyGameObject)
            Destroy(gameObject);
    }

    /// Start the timer.
    void Start()
    {
        StartCoroutine(timer());
    }
}
