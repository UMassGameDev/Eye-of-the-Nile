/**************************************************
Put this script on any object you want to automatically despawn after a certain amount of time.

Documentation updated 1/29/2024
**************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DespawnTimer : MonoBehaviour
{
    public float seconds = 10f;
    public bool destroyGameObject = true;
    public UnityEvent onDespawn;

    IEnumerator timer()
    {
        yield return new WaitForSeconds(seconds);
        onDespawn?.Invoke();
        if (destroyGameObject)
            Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(timer());
    }
}
