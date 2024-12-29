using UnityEngine;

/** \brief
Functionality of ObjectHealth for bosses.
This script inherits from ObjectHealth. What changed:
- This script updates how full the boss healthbar is.
- This script sets the display name of the boss above the healthbar.

This script DOES NOT control when the boss healthbar is visible, that is up to the boss behavior scripts.

Documentation updated 12/27/2024
\author Alexander Art
*/
public class BossHealth : ObjectHealth
{
    /// Reference to the boss healthbar HUD script.
    [SerializeField] public BossHealthbarHUD bossHealthbar;
    
    /// The display name of this boss that will appear above the healthbar.
    [SerializeField] protected string bossDisplayName;

    /// Used for checking when the health of the boss changes.
    private int previousHealth;

    /// Set the name on the boss healthbar and duplicate ObjectHealth's Start() because this overrides it.
    void Start()
    {
        // Set the name on the boss healthbar.
        bossHealthbar.SetDisplayName(bossDisplayName);

        // This function overrides the ObjectHealth Start(), so this is a duplicate of what it does:
        currentHealth = maxHealth;
    }

    /// Check for changes in the boss's health and update the healthbar.
    void Update()
    {
        if (previousHealth != currentHealth) // When the health changes.
        {
            previousHealth = currentHealth;
            bossHealthbar.SetHealthbarPercentage((float)currentHealth / (float)maxHealth);
        }
    }
}
