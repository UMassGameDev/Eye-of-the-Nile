using UnityEngine;

/** \brief
This script is a work in progress. It will control Geb's movement and bossroom mechanics.

Documentation updated 12/27/2024
\author Alexander Art
*/
public class GebController : MonoBehaviour
{
    /// Reference to Geb's health script, used for detecting when Geb reaches certain health thresholds.
    [SerializeField] protected BossHealth bossHealth;
    /// Reference to the boss healthbar HUD, used for making it appear and disappear.
    protected BossHealthbarHUD healthbar;
    /// Reference to the player object.
    protected GameObject player;

    /// (Probably temporary) radius around the boss that the player must be within for the boss's healthbar to be visible.
    protected float healthbarVisibleRadius = 20f;

    /// Set references to boss healthbar (already assigned in boss health script) and player GameObject.
    void Awake()
    {
        healthbar = bossHealth.bossHealthbar;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Make the healthbar visisble if Geb has health and is near the player.
        healthbar.SetHealthbarVisible(bossHealth.currentHealth > 0 && Vector2.Distance(transform.position, player.transform.position) < healthbarVisibleRadius);
    }
}
