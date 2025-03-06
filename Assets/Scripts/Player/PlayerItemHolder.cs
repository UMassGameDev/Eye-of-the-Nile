using System;
using System.Collections;
using UnityEngine;

/** \brief
Remembers the amount of health potions the player has, and uses the potions when potionHotkey is pressed.

Documentation updated 3/5/2024
\author Stephen Nuttall
*/
public class PlayerItemHolder : MonoBehaviour
{
    /// The hot key for using a potion.
    [SerializeField] KeyCode potionHotkey;
    /// The amount a potion should heal.
    [SerializeField] int potionHealAmount = 20;
    /// The maximum amount of potions the player can have.
    [SerializeField] int maxPotionCount = 6;
    /// The length of the cooldown after using a potion.
    [SerializeField] float cooldownLength = 3f;

    /// The amount of potions the player currently has.
    int potionCount = 0;
    /// Whether or not using the potion is on cooldown.
    bool onCooldown = false;

    /// Triggers when the amount of potions changes.
    public static event Action<int> potionCountChanged;
    /// Triggers when a potion is used.
    public static event Action<int> potionUsed;

    /// On Awake, get the potion count from the DataManager.
    void Awake()
    {
        DataManager dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
        potionCount = dataManager.GetHealthPotionCount();
        potionCountChanged?.Invoke(potionCount);
    }

    /// Subscribes to the PotionPickedUp event.
    void OnEnable()
    {
        HealthPotion.potionPickedUp += AddPotion;
    }

    /// Unsubscribes from the PotionPickedUp event.
    void OnDisable()
    {
        HealthPotion.potionPickedUp -= AddPotion;
    }

    /// Adds a potion unless the player currently has the maximum amount.
    void AddPotion()
    {
        if (potionCount < maxPotionCount)
        {
            potionCount++;
            potionCountChanged?.Invoke(potionCount);
        }
    }

    /// Every frame, use a potion if the user can and wants to.
    void Update()
    {
        if (!onCooldown && potionCount > 0 && Input.GetKey(potionHotkey))
        {
            potionCount--;

            potionUsed?.Invoke(potionHealAmount);
            potionCountChanged?.Invoke(potionCount);

            StartCoroutine(Cooldown());
        }
    }

    /// Set onCooldown to true, wait cooldownLength, then set onCooldown to false.
    IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(cooldownLength);
        onCooldown = false;
    }
}
