using UnityEngine;

/** \brief
This script goes on the ability cooldown UI elements and has function(s) for animating it.

Documentation updated 3/29/2025
\author Alexander Art
*/
public class CooldownVisualUI : MonoBehaviour
{
    /// Reference to the rectTransform that this script resizes as the cooldown visual.
    [SerializeField] protected RectTransform rectTransform;

    void Start()
    {
        // The cooldown boxes should start off empty (ability not on cooldown).
        SetFillPercentage(0f);
    }

    /// Resizes the rectTransform to the given percentage amount.
    public void SetFillPercentage(float fillPercentage)
    {
        if (!float.IsNaN(fillPercentage))
        {
            rectTransform.localScale = new Vector3(transform.localScale.x, fillPercentage, transform.localScale.z);
        }
    }
}
