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

    [SerializeField] TMP_Text soulCostText;
    [SerializeField] TMP_Text godsoulCostText;

    /// Set references.
    void Awake()
    {
        dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }

    public void OpenConfirmationPanel()
    {
        gameObject.SetActive(true);
    }

    public void CloseConfirmationPanel()
    {
        gameObject.SetActive(false);
    }
}
