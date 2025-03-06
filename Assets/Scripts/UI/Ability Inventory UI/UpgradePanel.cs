using UnityEngine;
using UnityEngine.UI;
using TMPro;

/** \brief
This script goes on the upgrade panel in the details panel of the ability inventory and has functions for handling ability upgrades.

Documentation updated 2/7/2025
\author Alexander Art
*/
public class UpgradePanel : MonoBehaviour
{
    /// Reference to the data manager.
    private DataManager dataManager;
    /// Reference to details panel.
    [SerializeField] private DetailsPanel detailsPanel;
    /// Reference to confirmation panel.
    [SerializeField] private ConfirmationPanel confirmationPanel;
    /// Reference to \ref Scriptables_AbilityInventory.
    [SerializeField] AbilityInventory abilityInventory;
    /// Reference to the ability inventory UI.
    [SerializeField] AbilityInventoryUI abilityInventoryUI;
    /// Reference to warning message popup when the player attempts an invalid upgrade.
    [SerializeField] GameObject warningMessage;

    [SerializeField] TMP_Text soulCostText;
    [SerializeField] TMP_Text godsoulCostText;

    /// Set references.
    void Awake()
    {
        dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
    }

    /// If the player has enough resources and the ability can be upgraded, open a confirmation box asking to upgrade the ability.
    public void OnUpgradeButtonPressed()
    {
        // Get the ability info for the selected ability on the details panel.
        BaseAbilityInfo abilityInfo = abilityInventory.GetAbilitySet(detailsPanel.dataForSelectedItem.abilityIndex);

        // Check if the ability is already at its maximum level.
        if (abilityInfo.abilityLevel < abilityInfo.maxLevel)
        {
            // Get cost for next ability upgrade.
            int soulCost = abilityInfo.upgradeSoulCosts[abilityInfo.abilityLevel - 1];
            int godsoulCost = abilityInfo.upgradeGodsoulCosts[abilityInfo.abilityLevel - 1];

            // If the player has enough currency, upgrade the ability and spend the currency.
            if (dataManager.GetSouls() >= soulCost && dataManager.GetGodSouls() >= godsoulCost)
            {
                confirmationPanel.OpenConfirmationPanel();
            }
            else
            {
                Instantiate(warningMessage, this.transform).GetComponent<TMP_Text>().SetText("Not enough currency!");
            }
        }
        else
        {
            Instantiate(warningMessage, this.transform).GetComponent<TMP_Text>().SetText("Ability already maxed!");
        }
    }

    /// Upgrades whichever ability is open in the details panel. DOES NOT CHECK FOR VALID RESOURCES!
    public void UpgradeSelectedAbility()
    {
        // Get the ability info for the selected ability on the details panel.
        BaseAbilityInfo abilityInfo = abilityInventory.GetAbilitySet(detailsPanel.dataForSelectedItem.abilityIndex);

        // Get cost for next ability upgrade.
        int soulCost = abilityInfo.upgradeSoulCosts[abilityInfo.abilityLevel - 1];
        int godsoulCost = abilityInfo.upgradeGodsoulCosts[abilityInfo.abilityLevel - 1];

        // Upgrade the ability.
        abilityInfo.UpgradeAbility();

        // Spend the player's currency.
        dataManager.SubtractSouls(soulCost);
        dataManager.SubtractGodSouls(godsoulCost);

        // Refresh the ability inventory UI to reflect the upgrade.
        abilityInventoryUI.UpdateActiveAbilities();
        abilityInventoryUI.InitializeSlots();
        detailsPanel.Initialize(detailsPanel.dataForSelectedItem);
        confirmationPanel.CloseConfirmationPanel();
    }

    /// Update the text beside the upgrade button to match the cost for the next ability upgrade.
    public void UpdateCostTextboxes()
    {
        // Get the ability info for the selected ability on the details panel.
        BaseAbilityInfo abilityInfo = abilityInventory.GetAbilitySet(detailsPanel.dataForSelectedItem.abilityIndex);

        // If the ability is below its maximum level, display the cost.
        // If the ability is at its maximum level, display "MAX"
        if (abilityInfo.abilityLevel < abilityInfo.maxLevel)
        {
            // Get cost for next ability upgrade.
            int soulCost = abilityInfo.upgradeSoulCosts[abilityInfo.abilityLevel - 1];
            int godsoulCost = abilityInfo.upgradeGodsoulCosts[abilityInfo.abilityLevel - 1];

            // Set text.
            soulCostText.SetText($"{soulCost}");
            godsoulCostText.SetText($"{godsoulCost}");
        }
        else
        {
            // Set text.
            soulCostText.SetText("MAX");
            godsoulCostText.SetText("MAX");
        }
    }
}
