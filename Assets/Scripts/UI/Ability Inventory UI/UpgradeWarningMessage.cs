using UnityEngine;
using TMPro;

/** \brief
This script animates and despawns a warning message that appears in the Skyhub when the player attempts to upgrade an 
ability that is already at its maximum level or when the player does not have enough currency.
(This script could probably be replaced by an animator.)

Documentation updated 2/7/2025
\author Alexander Art
*/
public class UpgradeWarningMessage : MonoBehaviour
{
    /// Reference to the text of the warning message object.
    private TMP_Text textMeshPro;

    /// The duration of the first part of the warning message's animation (spawning animation).
    protected float raiseTime = 0.25f;
    /// The duration of the middle part of the warning message's animation.
    protected float hoverTime = 1f;
    /// The duration of the last part of the warning message's animation (disappearing animation).
    protected float fallTime = 0.5f;
    /// How far the warning message raises in the first part of the animation.
    protected float raiseDistance = 80f;
    /// How far the warning message falls in the last part of the animation.
    protected float fallDistance = 100f;

    /// The game time at which this object was instantiated.
    private float timeSpawned;

    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
        timeSpawned = Time.unscaledTime;
    }

    void Update()
    {
        // First part of the animation.
        if (Time.unscaledTime - timeSpawned < raiseTime)
        {
            // Make the text larger.
            transform.localScale = new Vector3((Time.unscaledTime - timeSpawned) / raiseTime, (Time.unscaledTime - timeSpawned) / raiseTime, 1f);
            // Make the text fade in.
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, (Time.unscaledTime - timeSpawned) / raiseTime);
            // Make the text move up.
            transform.position = new Vector3(transform.position.x, transform.position.y + raiseDistance / raiseTime * Time.unscaledDeltaTime, transform.position.z);
        }
        // Middle part of the animation.
        else if (Time.unscaledTime - timeSpawned < raiseTime + hoverTime) {}
        // Third part of the animation.
        else if (Time.unscaledTime - timeSpawned < raiseTime + hoverTime + fallTime)
        {
            // Make the text fade out.
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1f - (Time.unscaledTime - timeSpawned - raiseTime - hoverTime) / raiseTime);
            // Make the text move down.
            transform.position = new Vector3(transform.position.x, transform.position.y - fallDistance / raiseTime * Time.unscaledDeltaTime, transform.position.z);
        }
        // Animation over.
        else
        {
            Destroy(gameObject);
        }
    }
}
