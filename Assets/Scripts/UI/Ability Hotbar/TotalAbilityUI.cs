using UnityEngine;
using UnityEngine.UI;

/** \brief
Governs the ability hotbar in the bottom left corner of the screen (\ref Prefabs_Abilities_AbilitySlots).
Updates the display ActiveAbilityData when the icons are moved or changed (including on first load).

Documentation updated 9/18/2024
\author Roy Pascual
\note This is a temporary feature and in the future you will not be able to drag the icons around without going to the Skyhub.
In the final version of this script, it will only be responsible for displaying the contents of ActiveAbilityData, not changing it.
*/
public class TotalAbilityUI : MonoBehaviour
{
    /// Reference to the data manager.
    DataManager dataManager;
    /// Reference to \ref Scriptables_ActiveAbilties, which stores all the ability info of the currently equipped abilities.
    public ActiveAbilityData equippedAbilities;
    /// Reference to \ref Prefabs_Abilities_AbilitySlotUI. One will be created for each ability type.
    public AbilitySlotUI abilitySlotUIPrefab;
    /// Reference to \ref Prefabs_Abilities_AbilityImageUI. One will be created for each equipped ability.
    public AbilityImageUI abilityImageUIPrefab;
    /// List of ability slots (AbilitySlotUI objects).
    AbilitySlotUI[] abilitySlotsUI;
    /// The maximum amount of ability slots.
    const int MAX_SLOTS = 4;

    /// \brief Shortly after the object is created or re-enabled, subscribe InitializeAllSlots() to AbilityInventoryUI.abilityInventoryClosed.
    /// This will re-initalize all slots when the ability inventory UI closes.
    void OnEnable()
    {
        AbilityInventoryUI.abilityInventoryClosed += InitializeAllSlots;
    }

    /// When the object is disabled, unsubscribe InitializeAllSlots() to AbilityInventoryUI.abilityInventoryClosed.
    void OnDisable()
    {
        AbilityInventoryUI.abilityInventoryClosed -= InitializeAllSlots;
    }

    /// This function is subscribed to the Drop event of each AbilitySlotUI. When an ability is dropped into that slot, refresh then display it.
    public void SlotDropListener(int slotNumber)
    {
        RefreshAbilityAtSlot(slotNumber);
        DisplayAbilityAtSlot(slotNumber);
    }

    /// Retrieves the ability at the given slot and updates \ref Scriptables_ActiveAbilties to reflect any changes.
    void RefreshAbilityAtSlot(int slotNumber)
    {
        AbilitySlotUI currentSlot = abilitySlotsUI[slotNumber];
        if (currentSlot.CurAbilityImageUI == null)
            equippedAbilities.SetAbilityAt(slotNumber, null);
        else
            equippedAbilities.SetAbilityAt(slotNumber, currentSlot.CurAbilityImageUI.CurAbilityInfo);
    }

    /// \brief Sets the sprite of the ability icon in each slot to the corresponding ability's sprite.
    /// If no icon is currently in that slot, one is created.
    void DisplayAbilityAtSlot(int slotNumber)
    {
        // if there's no ability in this slot, display nothing.
        if (equippedAbilities.AbilityAt(slotNumber) == null)
            return;
        
        // get the icons for the ability and the category of ability (offense, defense, utility, passive), as well as the slot we want to change.
        Sprite overlapSprite = equippedAbilities.AbilityAt(slotNumber).overlapIcon;
        Sprite currentSprite = equippedAbilities.AbilityAt(slotNumber).abilityIcons[slotNumber];
        AbilitySlotUI currentSlot = abilitySlotsUI[slotNumber];

        // if there's no icon in this slot, create one
        if (currentSlot.CurAbilityImageUI == null)
        {
            currentSlot.CurAbilityImageUI = Instantiate(abilityImageUIPrefab, currentSlot.transform, false);
            currentSlot.CurAbilityImageUI.CurImage.sprite = overlapSprite;
            currentSlot.CurAbilityImageUI.transform.Find("AbilitySubIconUI").GetComponent<Image>().sprite = currentSprite;
            currentSlot.CurAbilityImageUI.CurAbilityInfo = equippedAbilities.AbilityAt(slotNumber);
        }
        else
        {
            currentSlot.CurAbilityImageUI.CurImage.sprite = overlapSprite;
            currentSlot.CurAbilityImageUI.transform.Find("AbilitySubIconUI").GetComponent<Image>().sprite = currentSprite;
        }
    }

    /// Creates all slots in the UI (destroying them first if they already exist), then sets the slotIDs of each slot and subscribes to each AbilitySlotUI.
    void InitializeAllSlots()
    {
        if (abilitySlotsUI != null)
        {
            foreach(AbilitySlotUI abilitySlot in abilitySlotsUI)
            {
                abilitySlot.UnsubscribeFromDrop(this);
                Destroy(abilitySlot.gameObject);
            }
        }
        abilitySlotsUI = new AbilitySlotUI[MAX_SLOTS];
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            abilitySlotsUI[i] = Instantiate(abilitySlotUIPrefab, transform, false);
            abilitySlotsUI[i].slotID = i;
            abilitySlotsUI[i].SubscribeToDrop(this);
            if (equippedAbilities.AbilityAt(i) != null)
            {
                DisplayAbilityAtSlot(i);
            }
        }
    }

    /// Runs SetFillPercentage in CooldownVisualUI for the given slot.
    public void UpdateSlotCooldownVisual(int slot, float fillPercentage)
    {
        abilitySlotsUI[slot].CurCooldownVisualUI.SetFillPercentage(fillPercentage);
    }

    /// Set reference to the DataManager.
    void Awake()
    {
        dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
    }

    /// Run InitializeAllSlots() when the object is first created and if the player has unlocked abilities yet.
    void Start()
    {
        if (dataManager.abilitiesUnlocked == true)
        {
            InitializeAllSlots();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
