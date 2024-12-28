using UnityEngine;
using TMPro;

/** \brief
This script provides functions for other scripts to update the boss's healthbar on the Canvas.
There are functions for:
- Setting the health filled percentage.
- Setting the boss's display name.
- Setting the healthbar's visibility.

Documentation updated 12/27/2024
\author Alexander Art
\todo Create healthbar animations for when the boss spawns, takes damage, or gets defeated.
*/
public class BossHealthbarHUD : MonoBehaviour
{
    /// Reference to the healthbar underside, which is what the healthbar takes the shape of.
    [SerializeField] protected RectTransform healthbarUnderside;
    /// Reference to the red healthbar part of this object that shrinks as the boss takes damage.
    [SerializeField] protected RectTransform healthbar;
    /// Reference to the TextMeshPro above the healthbar that displays the boss's name.
    [SerializeField] protected TMP_Text bossNameText;

    /// <summary>
    /// Sets the healthbar's width and position to be filled a certain percentage.
    /// </summary>
    /// <param name="healthbarPercentage">The percentage full the healthbar will be set to.</param>
    public void SetHealthbarPercentage(float healthbarPercentage)
    {
        // Set the healthbar's width to the underside width times the percentage of health remaining.
        healthbar.sizeDelta = new Vector2(healthbarUnderside.rect.width * healthbarPercentage, healthbar.sizeDelta.y);
        // Update the healthbar's x position to be left-aligned.
        // Steps:
        // - Get the width of the missing health: healthbarUnderside.rect.width * (1 - healthbarPercentage) * healthbar.localScale.x
        //      - (1 - healthbarPercentage) is the percentage of missing health.
        //      - The actual width of the health bar is affected by the its scale, but the position isn't,
        //        so this part needs to be multiplied by the scale (healthbar.localScale.x).
        // - Divide by 2 because the healthbar's position is center-aligned by default,
        //   so it only needs to move left by half of the width of the missing health.
        // - Subtract all that from the intial x position of the healthbar to get where the healthbar needs to be.
        float newHealthbarX = healthbarUnderside.localPosition.x - (healthbarUnderside.rect.width * (1 - healthbarPercentage) * healthbar.localScale.x / 2);
        healthbar.localPosition = new Vector2(newHealthbarX, healthbar.localPosition.y);
    }

    /// <summary>
    /// Changes the text above the healthbar.
    /// </summary>
    /// <param name="bossName">The string that will be displayed.</param>
    public void SetDisplayName(string bossName)
    {
        bossNameText.text = bossName;
    }

    public void SetHealthbarVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }
}