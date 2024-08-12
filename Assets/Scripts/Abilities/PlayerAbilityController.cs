/**************************************************
Handles player input related to activating abilities and manages AbilityOwner objects for each of the 4 active abilities.

Documentation updated 8/11/2024
**************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAbilityController : MonoBehaviour
{
    public ActiveAbilityData activeAbilities;  // Reference to the ActiveAbilityData object.
    public List<UnityEvent> abilityEvents;

    private const int MAX_ABILITIES = 4;
    Dictionary<int, KeyCode> intToKey = new Dictionary<int, KeyCode>()
    {
        { 0, KeyCode.Alpha1 },
        { 1, KeyCode.Alpha2 },
        { 2, KeyCode.Alpha3 },
        { 3, KeyCode.Alpha4 }
    };
    Dictionary<KeyCode, bool> AbilityInputs;
    Dictionary<KeyCode, AbilityOwner> StoredAbilities;
    List<KeyCode> keyList;

    // This ability is subscribed to the AbilityOwner.ChargeUp event, and triggers the AbilityOwner.ChargingUp() coroutine.
    public void UseAbilityChargingUp(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.ChargingUp());
    }

    // This ability is subscribed to the AbilityOwner.CoolDown event, and triggers the AbilityOwner.CoolingDown() coroutine.
    public void UseAbilityCoolingDown(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.CoolingDown());
    }

    // This ability is subscribed to the AbilityOwner.AbilityUpdate event, and triggers the AbilityOwner.UpdateWithinDuration() coroutine.
    public void UseAbilityUpdate(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.UpdateWithinDuration());
    }

    // Subscribes a function to each event in AbilityOwner to run a corresponding conroutine in AbilityOwner.
    void SubscribeToAbility(AbilityOwner someAbility)
    {
        someAbility.ChargeUp += UseAbilityChargingUp;
        someAbility.CoolDown += UseAbilityCoolingDown;
        someAbility.AbilityUpdate += UseAbilityUpdate;
    }

    // Unsubscribes from each event in AbilityOwner
    void UnsubscribeFromAbility(AbilityOwner someAbility)
    {
        someAbility.ChargeUp -= UseAbilityChargingUp;
        someAbility.CoolDown -= UseAbilityCoolingDown;
        someAbility.AbilityUpdate -= UseAbilityUpdate;
    }

    // Attempts to change the AbilityOwner for the given key.
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

    // Refreshes the AbilityOwner.abilityInfo for all slots at the defined positions to match up with the active abilities.
    void RefreshSlots(int[] slotNumbers)
    {
        foreach (int slotNum in slotNumbers)
        {
            StoredAbilities[intToKey[slotNum]].abilityInfo =
                activeAbilities.AbilityAt(slotNum);
        }
    }

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
