/**************************************************
Displays the correct information in the details pop-up window in the ability inventory.
When you click the details button, this script makes sure the correct details are shown.

Documentation updated 4/15/2024
**************************************************/
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetailsPanel : MonoBehaviour
{
    [SerializeField] TMP_Text godName;
    [SerializeField] TMP_Text level;
    [SerializeField] Image godIcon;

    public void Initialize(AbilityInventoryItemData itemData)
    {
        godName.text = itemData.abilityName;
        godIcon.sprite = itemData.sprite;

        if (itemData.abilityLevel == 0) {
            level.text = "Locked";
        } else {
            level.text = "Level " + itemData.abilityLevel;
        }
    }
}
