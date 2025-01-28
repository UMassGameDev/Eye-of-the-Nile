using UnityEngine;
using System.Collections;

/** \brief
A simple effect applied to some fire obstacles. The object is scaled up and down between the given
minimum and maximum values.

Documentation updated 1/27/2025
\author Jiho Lee
*/
public class FireObstacleEffect : MonoBehaviour
{
    /// The minimum size the object will be scaled to.
    public float minScale = 0.8f;
    /// The maximum size the object will be scaled to.
    public float maxScale = 1.2f;
    /// The speed at which the object will be scaled.
    public float pulseSpeed = 2.0f;

    /// Starts the pulse coroutine.
    private void Start()
    {
        StartCoroutine(Pulse());
    }

    /// Cycles between scaling the object down towards minScale, back up towards maxScale, and so on.
    private IEnumerator Pulse()
    {
        while (true)
        {
            // Scale up
            for (float t = 0; t < 1; t += Time.deltaTime * pulseSpeed)
            {
                float scale = Mathf.Lerp(minScale, maxScale, t);
                transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }

            // Scale down
            for (float t = 0; t < 1; t += Time.deltaTime * pulseSpeed)
            {
                float scale = Mathf.Lerp(maxScale, minScale, t);
                transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }
        }
    }
}
