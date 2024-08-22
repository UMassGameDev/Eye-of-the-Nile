using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*!<summary>
Handles player input related to activating abilities and manages AbilityOwner objects for each of the 4 active abilities.

Documentation updated 8/11/2024
</summary>
\author Roy Pascual*/
public class PlayerAbilityController : MonoBehaviour
{
    /// \brief Reference to the ActiveAbilityData object.
    public ActiveAbilityData activeAbilities;
    /// \brief Runs the [i]th event when the [i]th ability is activated. Max size of 4 (0 - 3).
    public List<UnityEvent> abilityEvents;

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
    Dictionary<KeyCode, AbilityOwner> StoredAbilities;
    /// \brief List of each of the 4 ability keybinds (1, 2, 3, and 4).
    List<KeyCode> keyList;

    /// <summary>
    /// This ability is subscribed to the AbilityOwner.ChargeUp event, and triggers the AbilityOwner.ChargingUp() coroutine.
    /// </summary>
    /// <param name="abilityOwner"></param>
    public void UseAbilityChargingUp(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.ChargingUp());
    }

    /// <summary>
    /// This ability is subscribed to the AbilityOwner.CoolDown event, and triggers the AbilityOwner.CoolingDown() coroutine.
    /// </summary>
    /// <param name="abilityOwner"></param>
    public void UseAbilityCoolingDown(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.CoolingDown());
    }

    /// <summary>
    /// This ability is subscribed to the AbilityOwner.AbilityUpdate event, and triggers the AbilityOwner.UpdateWithinDuration() coroutine.
    /// </summary>
    /// <param name="abilityOwner"></param>
    public void UseAbilityUpdate(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.UpdateWithinDuration());
    }

    /// <summary>
    /// Subscribes a function to each event in AbilityOwner to run a corresponding conroutine in AbilityOwner.
    /// </summary>
    /// <param name="someAbility"></param>
    void SubscribeToAbility(AbilityOwner someAbility)
    {
        someAbility.ChargeUp += UseAbilityChargingUp;
        someAbility.CoolDown += UseAbilityCoolingDown;
        someAbility.AbilityUpdate += UseAbilityUpdate;
    }

    /// <summary>
    /// Unsubscribes from each event in AbilityOwner
    /// </summary>
    /// <param name="someAbility"></param>
    void UnsubscribeFromAbility(AbilityOwner someAbility)
    {
        someAbility.ChargeUp -= UseAbilityChargingUp;
        someAbility.CoolDown -= UseAbilityCoolingDown;
        someAbility.AbilityUpdate -= UseAbilityUpdate;
    }

    /// <summary>
    /// Attempts to change the AbilityOwner for the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="newAbility"></param>
    void TryChangeAbility(KeyCode key, AbilityOwner newAbility)
    {
        // Get stored ability
        AbilityOwner currentAbility = null;
        if (StoredAbilities.ContainsKey(key))
            currentAbility = StoredAbilities[key];

        // If the stored ability is not null, unsubscribe from the stored ability
        if (currentAbility != null)
            UnsubscribeFromAbility(currentAbility);
        
        SubscribeToAbility(newAbility);  // Subscribe to the new ability
        StoredAbilities[key] = newAbility;  // Store the new ability in the Dictionary
    }

    /// <summary>
    /// Refreshes the AbilityOwner.abilityInfo for all slots at the defined positions to match up with the active abilities.
    /// </summary>
    /// <param name="slotNumbers"></param>
    void RefreshSlots(int[] slotNumbers)
    {
        foreach (int slotNum in slotNumbers)
        {
            StoredAbilities[intToKey[slotNum]].abilityInfo =
                activeAbilities.AbilityAt(slotNum);
        }
    }

    /// <summary>
    /// Initializes AbilityInputs and keyList. Initializes StoredAbilities and adds a new AbilityOwner for each keycode.
    /// </summary>
    void Awake()
    {
        // initialize AbilityInputs and keyList
        AbilityInputs = new Dictionary<KeyCode, bool>();
        keyList = new List<KeyCode>
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4
        };
        foreach(KeyCode keyCheck in keyList)
        {
            AbilityInputs.Add(keyCheck, false);
        }

        // initialize StoredAbilities and add a new AbilityOwner for each keycode.
        if (StoredAbilities == null)
        {
            StoredAbilities = new Dictionary<KeyCode, AbilityOwner>();
        }
        for (int i = 0; i < MAX_ABILITIES; i++)
        {
            AbilityOwner newAbility = new AbilityOwner(transform,
                abilityEvents[i],
                activeAbilities.AbilityAt(i));
            TryChangeAbility(intToKey[i], newAbility);
        }
    }

    /// <summary>
    /// Check if any of the ability keybinds are pressed. If so, activate the corresponding ability.
    /// Check if a change to ActiveAbilityData was made. If so, refresh the AbilityOwners that were affected.
    /// </summary>
    void Update()
    {
        // If any of the ability keybinds are pressed, activate the corresponding ability.
        foreach (KeyCode keyCheck in keyList)
        {
            AbilityInputs[keyCheck] = Input.GetKeyDown(keyCheck);
            if (AbilityInputs[keyCheck]
                && StoredAbilities[keyCheck] != null
                && StoredAbilities[keyCheck].abilityInfo != null)
                StoredAbilities[keyCheck].ActivateAbility();
        }
        // If queueRefresh is true, a change to active abilities was made.
        // Refresh the AbilityOwners that were changed.
        if (activeAbilities.QueueRefresh)
        {
            RefreshSlots(activeAbilities.RefreshSlots.ToArray());
            activeAbilities.RefreshSlots.Clear();
            activeAbilities.QueueRefresh = false;
        }
    }
}
