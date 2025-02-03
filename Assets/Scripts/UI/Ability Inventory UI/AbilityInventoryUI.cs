using System;
using UnityEngine;
using UnityEngine.UI;

/** \brief
Handles the display and functionality of the ability inventory UI, accessed in the Skyhub.

Documentation updated 9/19/2024
\author Stephen Nuttall
*/
public class AbilityInventoryUI : MonoBehaviour
{
    /// Reference to \ref Scriptables_ActiveAbilties.
    [SerializeField] ActiveAbilityData activeAbilityData;
    /// Reference to \ref Scriptables_AbilityInventory.
    [SerializeField] AbilityInventory abilityInventory;

    /// Reference to the background panel of the ability inventory UI, which holds all the UI elements.
    [SerializeField] GameObject inventoryPanel;
    /// Reference to \ref Prefabs_Canvas. This will be disabled while the ability inventory is open.
    [SerializeField] GameObject mainCanvas;
    /// Reference to the details panel of the ability inventory UI, which holds the UI elements for the more details display.
    [SerializeField] GameObject detailsPanel;

    /// List of icons for the ability items.
    [SerializeField] Image[] abilityIcons;
    /// List of data for the ability items.
    [SerializeField] AbilityInventoryItemData[] iconData;

    /// List of slots at the top holding unused abilities.
    [SerializeField] AbilityInventorySlot[] abilityInventorySlots;
    /// List of slots at the bottom holding the currently used abilities.
    [SerializeField] AbilityInventorySlot[] activeAbilitySlots;

    /// Reference to \ref Scriptables_EmptyAbilityInfo.
    [SerializeField] EmptyAbilityInfo emptyAbilityInfo;

    /// Invoked when the ability inventory is opened.
    public static event Action abilityInventoryOpened;
    /// Invoked when the ability inventory is closed.
    public static event Action abilityInventoryClosed;
    /// Invoked when the slots in the ability inventory have been initialized.
    public static event Action abilityInventorySlotInitialized;
    
    /// True if the ability inventory is currently open.
    bool inventoryOpen = false;
    /// True if the ability details panel is currently open.
    bool detailsOpen = false;

    /// Run InitializeSlots().
    void Awake()
    {
        InitializeSlots();
    }

    /// Disables the main canvas and enables the inventory panel, then runs InitializeSlots().
    public void OpenInventory()
    {
        inventoryOpen = true;

        inventoryPanel.SetActive(true);
        mainCanvas.SetActive(false);

        // pause the game
        Time.timeScale = 0f;

        InitializeSlots();

        abilityInventoryOpened?.Invoke();
    }

    /// Updates \ref Scriptables_ActiveAbilties, then enables the main canvas and disables the inventory panel.
    public void ExitInventory()
    {
        UpdateActiveAbilities();

        inventoryPanel.SetActive(false);
        mainCanvas.SetActive(true);
        
        ExitDetailsPanel();
        
        // unpause the game
        Time.timeScale = 1f;

        inventoryOpen = false;

        abilityInventoryClosed?.Invoke();
    }

    /// Opens the more details view under the given ability slot.
    public void OpenDetailsPanel(AbilityInventorySlot inventorySlot)
    {
        detailsPanel.SetActive(true);
        detailsPanel.GetComponent<DetailsPanel>().Initialize(inventorySlot.slotData);

        detailsOpen = true;
    }

    /// Closes the more details view.
    public void ExitDetailsPanel()
    {
        detailsPanel.SetActive(false);
        
        detailsOpen = false;
    }

    /// Runs every frame. Checks if the user has pressed escape or E. If so, closes the menu(s).
    void Update()
    {
        // If the inventory is open and the user presses escape, close the game
        if (inventoryOpen == true && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitInventory();
        }
        // If the details panel is open and the user presses E, it closes the details panel. If just the inventory is open, it closes the inventory.
        if (Input.GetKeyDown(KeyCode.E) && Time.deltaTime == 0f)
        {
            if (detailsOpen)
            {
                ExitDetailsPanel();
            }
            else if (inventoryOpen)
            {
                ExitInventory();
            }
        }
    }

    /// Fills out the AbilityInventoryItemData for each slot.
    void InitializeSlots()
    {
        // initialize ability inventory slots and their icons
        for (int i = 0; i < abilityInventorySlots.Length; i++)
        {
            if (abilityInventory.GetAbilitySet(i) != null)
            {
                // Each ability's icon and each inventory slot holds an AbilityInventoryItemData object.
                // This holds all the information about the ability that needs to be displayed.
                // It also holds what inventory slot that ability's icon is in before a drag and drop occurs.

                BaseAbilityInfo thisAbility = abilityInventory.GetAbilitySet(i);
                abilityIcons[i].sprite = thisAbility.overlapIcon;

                iconData[i].abilityName = thisAbility.abilityName;
                iconData[i].abilityLevel = thisAbility.abilityLevel;
                iconData[i].sprite = thisAbility.overlapIcon;
                iconData[i].quote = thisAbility.GetQuote();
                iconData[i].abilityIcons = thisAbility.abilityIcons;
                iconData[i].abilityNames = thisAbility.GetAllAbilityNames();
                iconData[i].abilityDescriptions = thisAbility.GetAllAbilityDescriptions();

                // store this data in the inventory slot
                abilityInventorySlots[i].slotData = iconData[i];

                // Store which slot the item is in on initialization
                iconData[i].currentSlot = abilityInventorySlots[i].GetSlotNum();

                iconData[i].abilityIndex = i;
            }

            abilityInventorySlotInitialized?.Invoke();
        }

        // move already active abilities to the active slot
        for (int i = 0; i < activeAbilitySlots.Length; i++)
        {
            // skip empty slots (null or emptyAbilityInfo)
            if (activeAbilityData.AbilityAt(i) != null && activeAbilityData.AbilityAt(i).abilityName != emptyAbilityInfo.abilityName) 
            {
                string currAbilityName = activeAbilityData.AbilityAt(i).abilityName;
                for (int n = 0; n < abilityInventorySlots.Length; n++)
                {
                    if (abilityInventorySlots[n].slotData.abilityName == currAbilityName)
                    {
                        iconData[n].MoveIcon(activeAbilitySlots[i].GetPosition());
                        activeAbilitySlots[i].slotData = abilityInventorySlots[n].slotData;
                        
                        // Correct which slot the item is in if it is already active on initialization
                        iconData[n].currentSlot = activeAbilitySlots[i].GetSlotNum();
                    }
                }
            }
            
        }
    }

    /// Update \ref Scriptables_ActiveAbilties to reflect any changes made in the ability inventory.
    void UpdateActiveAbilities()
    {
        // update active ability data with new active abilities
        for (int i = 0; i < activeAbilitySlots.Length; i++)
        {
            BaseAbilityInfo newAbilityInfo;

            if (activeAbilitySlots[i].slotData == null) {
                newAbilityInfo = emptyAbilityInfo;
            } else {
                newAbilityInfo = abilityInventory.GetAbilitySet(activeAbilitySlots[i].slotData.abilityName);
                if (newAbilityInfo == null || newAbilityInfo.abilityName == emptyAbilityInfo.abilityName)
                    newAbilityInfo = emptyAbilityInfo;
            }

            activeAbilityData.SetAbilityAt(i, newAbilityInfo);
            Debug.Log("Ability #" + i + " set to \"" + newAbilityInfo.abilityName + '\"');
        }
    }
}
