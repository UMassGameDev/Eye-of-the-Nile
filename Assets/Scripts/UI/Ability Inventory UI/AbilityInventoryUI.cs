/**************************************************
Handles the display and functionality of the ability inventory UI, accessed in the Skyhub.

Documentation updated 3/14/2024
**************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;

public class AbilityInventoryUI : MonoBehaviour
{
    [SerializeField] ActiveAbilityData activeAbilityData;
    [SerializeField] AbilityInventory abilityInventory;

    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject mainCanvas;
    [SerializeField] GameObject detailsPanel;

    [SerializeField] Image[] abilityIcons;
    [SerializeField] AbilityInventoryItemData[] iconData;

    [SerializeField] AbilityInventorySlot[] abilityInventorySlots;
    [SerializeField] AbilityInventorySlot[] activeAbilitySlots;

    [SerializeField] EmptyAbilityInfo emptyAbilityInfo;

    public static event Action abilityInventoryOpened;
    public static event Action abilityInventoryClosed;
    public static event Action abilityInventorySlotInitialized;
    
    bool inventoryOpen = false;

    void Awake()
    {
        InitializeSlots();
    }

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

    public void ExitInventory()
    {
        UpdateActiveAbilities();

        inventoryPanel.SetActive(false);
        mainCanvas.SetActive(true);
        
        // unpause the game
        Time.timeScale = 1f;

        inventoryOpen = false;

        abilityInventoryClosed?.Invoke();
    }

    public void OpenDetailsPanel(AbilityInventorySlot inventorySlot)
    {
        detailsPanel.SetActive(true);
        detailsPanel.GetComponent<DetailsPanel>().Initialize(inventorySlot.slotData);
    }

    public void ExitDetailsPanel()
    {
        detailsPanel.SetActive(false);
    }

    void Update()
    {
        // if the inventory is open and the user presses escape, close the game
        if (inventoryOpen == true && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitInventory();
        }
    }

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
                    }
                }
            }
            
        }
    }

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
