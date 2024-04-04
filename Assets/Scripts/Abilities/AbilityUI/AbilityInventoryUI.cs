/**************************************************
Handles the display and functionality of the ability inventory UI, accessed in the Skyhub.

Documentation updated 3/14/2024
**************************************************/
using UnityEngine;
using UnityEngine.UI;

public class AbilityInventoryUI : MonoBehaviour
{
    [SerializeField] ActiveAbilityData activeAbilityData;
    [SerializeField] AbilityInventory abilityInventory;

    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject mainCanvas;

    [SerializeField] Image[] abilityIcons;
    [SerializeField] AbilityInventoryItemData[] iconData;

    [SerializeField] AbilityInventorySlot[] abilityInventorySlots;
    [SerializeField] AbilityInventorySlot[] activeAbilitySlots;
    
    bool inventoryOpen = false;

    void Awake()
    {
        // initialize ability inventory slots and their icons
        for (int i = 0; i < abilityInventorySlots.Length; i++)
        {
            if (abilityInventory.GetAbilitySet(i) != null)
            {
                abilityInventorySlots[i].abilityName = abilityInventory.GetAbilitySet(i).abilityName;
                abilityIcons[i].sprite = abilityInventory.GetAbilitySet(i).overlapIcon;
                iconData[i].abilityName = abilityInventory.GetAbilitySet(i).abilityName;
            }
        }

        // move already active abilities to the active slot
        for (int i = 0; i < activeAbilitySlots.Length; i++)
        {
            string currAbilityName = activeAbilityData.AbilityAt(i).abilityName;
            for (int n = 0; n < abilityInventorySlots.Length; n++)
            {
                if (abilityInventorySlots[n].abilityName == currAbilityName)
                {
                    iconData[n].MoveIcon(activeAbilitySlots[i].GetPosition());
                    activeAbilitySlots[i].abilityName = currAbilityName;
                }
            }
        }
    }

    public void OpenInventory()
    {
        inventoryOpen = true;
        inventoryPanel.SetActive(true);
        mainCanvas.SetActive(false);

        // puase the game
        Time.timeScale = 0f;
    }

    public void ExitInventory()
    {
        UpdateActiveAbilities();
        inventoryPanel.SetActive(false);
        mainCanvas.SetActive(true);
        Time.timeScale = 1f;
        inventoryOpen = false;
    }

    void Update()
    {
        // if the inventory is open and the user presses escape, close the game
        if (inventoryOpen == true && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            ExitInventory();
        }
    }

    void UpdateActiveAbilities()
    {
        for (int i = 0; i < activeAbilitySlots.Length; i++)
        {
            BaseAbilityInfo newAbilityInfo = abilityInventory.GetAbilitySet(activeAbilitySlots[i].abilityName);
            activeAbilityData.SetAbilityAt(i, newAbilityInfo);
        }
    }
}
