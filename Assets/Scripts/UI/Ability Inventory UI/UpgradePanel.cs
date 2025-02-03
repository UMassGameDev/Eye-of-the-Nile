using UnityEngine;
using UnityEngine.UI;
using TMPro;

/** \brief
This script goes on the upgrade panel in the details panel of the ability inventory and has functions for handling ability upgrades.

Documentation updated 2/2/2025
\author Alexander Art
*/
public class UpgradePanel : MonoBehaviour
{
    /// Reference to the data manager.
    private DataManager dataManager;
    /// Reference to details panel.
    [SerializeField] private DetailsPanel detailsPanel;
    /// Reference to \ref Scriptables_AbilityInventory.
    [SerializeField] AbilityInventory abilityInventory;

    /// Set references.
    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    public void UpgradeSelectedAbility()
    {
        abilityInventory.GetAbilitySet(detailsPanel.dataForSelectedItem.abilityIndex).UpgradeAbility();;
    }
}
