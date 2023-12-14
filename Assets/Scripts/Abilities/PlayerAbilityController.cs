using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAbilityController : MonoBehaviour
{
    public ActiveAbilityData activeAbilities;
    public List<UnityEvent> abilityEvents;

    private const int MAX_ABILITIES = 4;
    public Dictionary<int, KeyCode> intToKey = new Dictionary<int, KeyCode>()
    {
        { 0, KeyCode.Alpha1 },
        { 1, KeyCode.Alpha2 },
        { 2, KeyCode.Alpha3 },
        { 3, KeyCode.Alpha4 }
    };
    public Dictionary<KeyCode, bool> AbilityInputs { get; set; }
    public Dictionary<KeyCode, AbilityOwner> StoredAbilities { get; set; }
    private List<KeyCode> keyList;

    // Starts a public Coroutine in the AbilityOwner object
    // This method subscribes to the ChargeUp event of each AbilityOwner
    public void UseAbilityChargingUp(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.ChargingUp());
    }

    // Starts a public Coroutine in the AbilityOwner object
    // This method subscribes to the CoolDown event of each AbilityOwner
    public void UseAbilityCoolingDown(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.CoolingDown());
    }

    public void UseAbilityUpdate(AbilityOwner abilityOwner)
    {
        StartCoroutine(abilityOwner.UpdateWithinDuration());
    }

    // Subscribes to both events in AbilityOwner
    void SubscribeToAbility(AbilityOwner someAbility)
    {
        someAbility.ChargeUp += UseAbilityChargingUp;
        someAbility.CoolDown += UseAbilityCoolingDown;
        someAbility.AbilityUpdate += UseAbilityUpdate;
    }

    // Unsubscribes to both events in AbilityOwner
    void UnsubscribeFromAbility(AbilityOwner someAbility)
    {
        someAbility.ChargeUp -= UseAbilityChargingUp;
        someAbility.CoolDown -= UseAbilityCoolingDown;
        someAbility.AbilityUpdate -= UseAbilityUpdate;
    }

    // Attempts to change an ability
    // If the stored ability is not null,
    // Unsubscribes from the stored ability
    // Subscribes to the new ability
    // Then stores the new ability in the Dictionary
    // If stored ability is null,
    // Subscribes then stores the new ability
    void TryChangeAbility(KeyCode key, AbilityOwner newAbility)
    {
        AbilityOwner currentAbility = null;
        if (StoredAbilities.ContainsKey(key))
            currentAbility = StoredAbilities[key];
        if (currentAbility != null)
        {
            UnsubscribeFromAbility(currentAbility);
            SubscribeToAbility(newAbility);
            StoredAbilities[key] = newAbility;
        }
        else
        {
            SubscribeToAbility(newAbility);
            StoredAbilities[key] = newAbility;
        }
    }

    // Refreshes all slots at the defined positions
    // To match up with the active abilities
    void RefreshSlots(int[] slotNumbers)
    {
        foreach (int slotNum in slotNumbers)
        {
            StoredAbilities[intToKey[slotNum]].abilityInfo =
                activeAbilities.AbilityAt(slotNum);
        }
    }

    // Changes ability at the current slot number to the new ability
    public void StoreAbilityInSlot(int slotNumber, AbilityOwner newAbility)
    {
        /*switch (slotNumber)
        {
            case 0:
                // StoredAbilities[KeyCode.Alpha1] = newAbility;
                TryChangeAbility(KeyCode.Alpha1, newAbility);
                // activeAbilities.offenseSlot = newAbility.abilityInfo;
                break;
            case 1:
                // StoredAbilities[KeyCode.Alpha2] = newAbility;
                TryChangeAbility(KeyCode.Alpha2, newAbility);
                // activeAbilities.defenseSlot = newAbility.abilityInfo;
                break;
            case 2:
                // StoredAbilities[KeyCode.Alpha3] = newAbility;
                TryChangeAbility(KeyCode.Alpha3, newAbility);
                // activeAbilities.utilitySlot = newAbility.abilityInfo;
                break;
            case 3:
                // StoredAbilities[KeyCode.Alpha4] = newAbility;
                TryChangeAbility(KeyCode.Alpha4, newAbility);
                // activeAbilities.passiveSlot = newAbility.abilityInfo;
                break;
            default:
                break;
        }*/
        TryChangeAbility(intToKey[slotNumber], newAbility);
    }

    void Awake()
    {
        AbilityInputs = new Dictionary<KeyCode, bool>();
        keyList = new List<KeyCode>();
        keyList.Add(KeyCode.Alpha1);
        keyList.Add(KeyCode.Alpha2);
        keyList.Add(KeyCode.Alpha3);
        keyList.Add(KeyCode.Alpha4);
        foreach(KeyCode keyCheck in keyList)
        {
            AbilityInputs.Add(keyCheck, false);
        }

        if (StoredAbilities == null)
        {
            StoredAbilities = new Dictionary<KeyCode, AbilityOwner>();
        }
        for (int i = 0; i < MAX_ABILITIES; i++)
        {
            AbilityOwner newAbility = new AbilityOwner(transform,
                abilityEvents[i],
                activeAbilities.AbilityAt(i));
            StoreAbilityInSlot(i, newAbility);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Checks against possible keys and activates the ability for the key
        foreach (KeyCode keyCheck in keyList)
        {
            AbilityInputs[keyCheck] = Input.GetKeyDown(keyCheck);
            if (AbilityInputs[keyCheck]
                && StoredAbilities[keyCheck] != null
                && StoredAbilities[keyCheck].abilityInfo != null)
                StoredAbilities[keyCheck].ActivateAbility();
        }
        // If queueRefresh is true, a change to active abilities was made
        if (activeAbilities.QueueRefresh)
        {
            RefreshSlots(activeAbilities.RefreshSlots.ToArray());
            activeAbilities.RefreshSlots.Clear();
            activeAbilities.QueueRefresh = false;
        }
    }
}
