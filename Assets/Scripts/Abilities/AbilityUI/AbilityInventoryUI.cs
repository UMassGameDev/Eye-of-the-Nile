/**************************************************
Handles the display and functionality of the ability inventory UI, accessed in the Skyhub.

Documentation updated 3/14/2024
**************************************************/
using UnityEngine;
using UnityEngine.UI;

public class AbilityInventoryUI : MonoBehaviour
{
    public AbilityInventory abilityInventory;
    public GameObject inventoryPanel;
    public GameObject mainCanvas;
    public GameObject[] inventorySlots;
    
    bool inventoryOpen = false;

    public void OpenInventory()
    {
        inventoryOpen = true;
        inventoryPanel.SetActive(true);
        mainCanvas.SetActive(false);

        // Set each slot icon to its corresponding ability icon
        int i = 0;
        foreach (GameObject slot in inventorySlots)
        {
            if (abilityInventory.AbilitySets[i] == null) {
                slot.SetActive(false);
                Debug.Log("Ability " + i + " in inventory isn't set. Slot disabled");
            } else {
                slot.SetActive(true);
                slot.transform.GetChild(1).GetComponent<Image>().sprite = abilityInventory.AbilitySets[i].overlapIcon;
            }
            i++;
        }

        // puase the game
        Time.timeScale = 0f;
    }

    public void ExitInventory()
    {
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
}
