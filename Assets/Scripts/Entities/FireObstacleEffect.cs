using UnityEngine;
using System.Collections;

public class FireObstacleEffect : MonoBehaviour
{
    public float minScale = 0.8f;  
    public float maxScale = 1.2f;  
    public float pulseSpeed = 2.0f; 

    private void Start()
    {
        StartCoroutine(Pulse());
    }

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
