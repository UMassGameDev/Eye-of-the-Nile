using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*!<summary>
Handles player input related to activating abilities and manages AbilityOwner objects for each of the 4 active abilities.

Documentation updated 8/11/2024
</summary>
\author Roy Pascual, Stephen Nuttall*/
public class PlayerAbilityController : MonoBehaviour
{
    /// \brief Reference to the data manager.
    DataManager dataManager;
    /// \brief Reference to the ActiveAbilityData object.
    public ActiveAbilityData activeAbilities;
    /// \brief Runs the [i]th event when the [i]th ability is activated. Max size of 4 (0 - 3).
    public List<UnityEvent> abilityEvents;

    /// \brief Reference to the TotalAbilityUI.
    private TotalAbilityUI totalAbilityUI;

    /// \brief Maximum number of active ability slots.
    private const int MAX_ABILITIES = 4;
    /// \brief Maps the ints 0 - 3 to the keyboard keys 1 - 4.
    Dictionary<int, KeyCode> intToKey = new Dictionary<int, KeyCode>()
    {
        { 0, KeyCode.Alpha1 },
        { 1, KeyCode.Alpha2 },
        { 2, KeyCode.Alpha3 },
        { 3, KeyCode.Alpha4 }
    };
    /// \brief Map of all 4 ability keys, and if they’re being pressed or not.
    Dictionary<KeyCode, bool> AbilityInputs;
    /// \brief Maps each ability key to an AbilityOwner, which contains some information about ability and handles charge ups/cooldowns.
    Dictionary<KeyCode, AbilityOwner> StoredAbilityOwners;
    /// \brief List of each of the 4 ability keybinds (1, 2, 3, and 4).
    List<KeyCode> keyList;

    /// \brief This ability is subscribed to the AbilityOwner.ChargeUp event, and triggers the AbilityOwner.ChargingUp() coroutine.
    /// See more information in \ref AbilityOwnerEvents.
    public void UseAbilityChargingUp(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.ChargingUp());
    }

    /// \brief This ability is subscribed to the AbilityOwner.CoolDown event, and triggers the AbilityOwner.CoolingDown() coroutine.
    /// See more information in \ref AbilityOwnerEvents.
    public void UseAbilityCoolingDown(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.CoolingDown());
    }

    /// \brief This ability is subscribed to the AbilityOwner.AbilityUpdate event, and triggers the AbilityOwner.UpdateWithinDuration() coroutine.
    /// See more information in \ref AbilityOwnerEvents.
    public void UseAbilityUpdate(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.UpdateWithinDuration());
    }

    /// Subscribes a function to each event in AbilityOwner to run a corresponding conroutine in AbilityOwner.
    void SubscribeToAbility(AbilityOwner someAbility)
    {
        someAbility.ChargeUp += UseAbilityChargingUp;
        someAbility.CoolDown += UseAbilityCoolingDown;
        someAbility.AbilityUpdate += UseAbilityUpdate;
    }

    /// Unsubscribes from each event in AbilityOwner
    void UnsubscribeFromAbility(AbilityOwner someAbility)
    {
        someAbility.ChargeUp -= UseAbilityChargingUp;
        someAbility.CoolDown -= UseAbilityCoolingDown;
        someAbility.AbilityUpdate -= UseAbilityUpdate;
    }

    /// \brief Attempts to change the AbilityOwner for the given key.
    /// Also runs OnEquipped(), and activates the passive ability in the last slot
    void TryChangeAbility(KeyCode key, AbilityOwner newAbility)
    {
        // Get stored ability
        AbilityOwner currentAbility = null;
        if (StoredAbilityOwners.ContainsKey(key))
            currentAbility = StoredAbilityOwners[key];

        // If the stored ability is not null, unsubscribe from the stored ability
        if (currentAbility != null)
            UnsubscribeFromAbility(currentAbility);

        SubscribeToAbility(newAbility);  // Subscribe to the new ability
        StoredAbilityOwners[key] = newAbility;  // Store the new ability in the Dictionary

        newAbility.abilityInfo.OnEquipped(newAbility);

        if (newAbility.abilityInfo.currentForm == AbilityForm.Passive)
        {
            newAbility.EnablePassive();
        }
    }

    /// \brief Refreshes the AbilityOwner.abilityInfo for all slots at the defined positions to match up with the active abilities.
    /// Also runs OnEquipped() and OnUnequipped, and activates or disables the passive ability in the last slot
    void RefreshSlots(int[] slotNumbers)
    {
        foreach (int slotNum in slotNumbers)
        {
            if (StoredAbilityOwners[intToKey[slotNum]].abilityInfo != activeAbilities.AbilityAt(slotNum))
            {
                StoredAbilityOwners[intToKey[slotNum]].abilityInfo.OnUnequipped(StoredAbilityOwners[intToKey[slotNum]]);
                StoredAbilityOwners[intToKey[slotNum]].abilityInfo = activeAbilities.AbilityAt(slotNum);
                StoredAbilityOwners[intToKey[slotNum]].abilityInfo.OnEquipped(StoredAbilityOwners[intToKey[slotNum]]);
            }

            if (slotNum < MAX_ABILITIES - 1)
            {
                StoredAbilityOwners[intToKey[slotNum]].DisablePassive();
            }
            else if (slotNum == MAX_ABILITIES - 1)
            {
                StoredAbilityOwners[intToKey[slotNum]].EnablePassive();
            }
        }
    }

    /// Initializes AbilityInputs and keyList. Initializes StoredAbilities and adds a new AbilityOwner for each keycode. Also sets a reference.
    void Awake()
    {
        // Set reference to dataManager.
        dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();

        // Set reference to totalAbilityUI.
        totalAbilityUI = GameObject.Find("AbilitySlots").GetComponent<TotalAbilityUI>();

        // initialize AbilityInputs and keyList
        AbilityInputs = new Dictionary<KeyCode, bool>();
        keyList = new List<KeyCode>
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4
        };
        foreach (KeyCode keyCheck in keyList)
        {
            AbilityInputs.Add(keyCheck, false);
        }

        // initialize StoredAbilities and add a new AbilityOwner for each keycode.
        if (StoredAbilityOwners == null)
        {
            StoredAbilityOwners = new Dictionary<KeyCode, AbilityOwner>();
        }
        for (int i = 0; i < MAX_ABILITIES; i++)
        {
            AbilityOwner newAbility = new AbilityOwner(transform,
                abilityEvents[i],
                activeAbilities.AbilityAt(i));
            TryChangeAbility(intToKey[i], newAbility);
        }
    }

    /// \brief Check if any of the ability keybinds are pressed. If so, activate the corresponding ability.
    /// Check if a change to ActiveAbilityData was made. If so, refresh the AbilityOwners that were affected.
    /// Update the cooldown visuals based on the remaining cooldown time.
    void Update()
    {
        // If the ability hotbar has been unlocked, check for player input to activate the abilities.
        if (dataManager.abilitiesUnlocked == true)
        {
            // If any of the ability keybinds are pressed, activate the corresponding ability (unless it's the passive ability).
            foreach (KeyCode keyCheck in keyList)
            {
                AbilityInputs[keyCheck] = Input.GetKeyDown(keyCheck);
                if (AbilityInputs[keyCheck]  // if this key is being pressed
                    && StoredAbilityOwners[keyCheck] != null  // and if this key has a slot assigned to it
                    && StoredAbilityOwners[keyCheck].abilityInfo != null  // and if there is a stored ability in this slot
                    && StoredAbilityOwners[keyCheck].abilityInfo.currentForm != AbilityForm.Passive  // and if this ability is not the passive ability
                    && Time.timeScale > 0.0f) // and time is not frozen (in menu)
                    StoredAbilityOwners[keyCheck].ActivateAbility();  // activate the ability.
            }
        }
        // If queueRefresh is true, a change to active abilities was made.
        // Refresh the AbilityOwners that were changed.
        if (activeAbilities.QueueRefresh)
        {
            RefreshSlots(activeAbilities.RefreshSlots.ToArray());
            activeAbilities.RefreshSlots.Clear();
            activeAbilities.QueueRefresh = false;
        }

        if (totalAbilityUI.gameObject.activeInHierarchy == true)
        {
            // Update the cooldown visuals based on the remaining cooldown time.
            for (int i = 0; i < MAX_ABILITIES; i++)
            {
                float cooldownTimeRemaining = StoredAbilityOwners[intToKey[i]].cooldownEnd - Time.time;
                float cooldownPercentage = cooldownTimeRemaining / StoredAbilityOwners[intToKey[i]].abilityInfo.cooldown;
                totalAbilityUI.UpdateSlotCooldownVisual(i, Math.Max(cooldownPercentage, 0f));
            }
        }
    }
}
