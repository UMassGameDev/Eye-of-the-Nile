using UnityEngine;
using UnityEngine.UI;
using TMPro;

/** \brief
This script goes on the confirmation panel after the upgrade button is pressed in the ability inventory.

Documentation updated 2/4/2025
\author Alexander Art
*/
public class ConfirmationPanel : MonoBehaviour
{
    /// Reference to the data manager.
    private DataManager dataManager;
    /// Reference to details panel.
    [SerializeField] private DetailsPanel detailsPanel;
    /// Reference to upgrade panel.
    [SerializeField] private UpgradePanel upgradePanel;
    /// Reference to \ref Scriptables_AbilityInventory.
    [SerializeField] AbilityInventory abilityInventory;
    /// Reference to the ability inventory UI.
    [SerializeField] AbilityInventoryUI abilityInventoryUI;

    [SerializeField] TMP_Text currentSoulText;
    [SerializeField] TMP_Text currentGodsoulText;
    [SerializeField] TMP_Text soulCostText;
    [SerializeField] TMP_Text godsoulCostText;
    [SerializeField] TMP_Text remainingSoulText;
    [SerializeField] TMP_Text remainingGodsoulText;

    /// Set references.
    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    public void OpenConfirmationPanel()
    {
        gameObject.SetActive(true);
        
        // Get the ability info for the selected ability.
        BaseAbilityInfo abilityInfo = abilityInventory.GetAbilitySet(detailsPanel.dataForSelectedItem.abilityIndex);

        // Set text for current soul count.
        currentSoulText.SetText($"{dataManager.GetSouls()}");
        currentGodsoulText.SetText($"{dataManager.GetGodSouls()}");

        // Set cost text for next ability upgrade.
        soulCostText.SetText($"{abilityInfo.upgradeSoulCosts[abilityInfo.abilityLevel - 1]}");
        godsoulCostText.SetText($"{abilityInfo.upgradeGodsoulCosts[abilityInfo.abilityLevel - 1]}");

        // Set text for remaining soul count.
        remainingSoulText.SetText($"{dataManager.GetSouls() - abilityInfo.upgradeSoulCosts[abilityInfo.abilityLevel - 1]}");
        remainingGodsoulText.SetText($"{dataManager.GetGodSouls() - abilityInfo.upgradeGodsoulCosts[abilityInfo.abilityLevel - 1]}");
    }

    public void CloseConfirmationPanel()
    {
        gameObject.SetActive(false);
    }
}
