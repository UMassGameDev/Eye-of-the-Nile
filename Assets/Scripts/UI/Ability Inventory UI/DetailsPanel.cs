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
    public AbilityInventoryItemData dataForSelectedItem;

    [SerializeField] TMP_Text godName;
    [SerializeField] TMP_Text level;
    [SerializeField] Image godIcon;
    [SerializeField] TMP_Text quote;

    [SerializeField] Image offenseIcon;
    [SerializeField] TMP_Text offenseName;
    [SerializeField] TMP_Text offenseDescription;

    [SerializeField] Image defenseIcon;
    [SerializeField] TMP_Text defenseName;
    [SerializeField] TMP_Text defenseDescription;

    [SerializeField] Image utilityIcon;
    [SerializeField] TMP_Text utilityName;
    [SerializeField] TMP_Text utilityDescription;

    [SerializeField] Image passiveIcon;
    [SerializeField] TMP_Text passiveName;
    [SerializeField] TMP_Text passiveDescription;

    [SerializeField] UpgradePanel upgradePanel;

    public void Initialize(AbilityInventoryItemData itemData)
    {
        dataForSelectedItem = itemData;

        godIcon.sprite = itemData.sprite;
        godName.text = itemData.abilityName;
        quote.text = itemData.quote;

        if (itemData.abilityLevel == 0) {
            level.text = "Locked";
        } else {
            level.text = "Level " + itemData.abilityLevel;
        }

        offenseIcon.sprite = itemData.abilityIcons[0];
        offenseName.text = itemData.abilityNames[0];
        offenseDescription.text = itemData.abilityDescriptions[0];

        defenseIcon.sprite = itemData.abilityIcons[1];
        defenseName.text = itemData.abilityNames[1];
        defenseDescription.text= itemData.abilityDescriptions[1];

        utilityIcon.sprite = itemData.abilityIcons[2];
        utilityName.text = itemData.abilityNames[2];
        utilityDescription.text = itemData.abilityDescriptions[2];

        passiveIcon.sprite = itemData.abilityIcons[3];
        passiveName.text = itemData.abilityNames[3];
        passiveDescription.text = itemData.abilityDescriptions[3];

        upgradePanel.UpdateCostTextboxes();
    }
}
