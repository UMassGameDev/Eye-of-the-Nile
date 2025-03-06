using UnityEngine;
using TMPro;

/** \brief
Updates the text display for how many health potions the player currently has.

Documentation updated 3/5/2024
\author Stephen Nuttall
*/
public class PotionCounter : MonoBehaviour
{
    /// Reference to the text that displays the amount of potions.
    [SerializeField] TMP_Text countText;

    /// On Awake, get the potion count from the DataManager.
    void Awake()
    {
        DataManager dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
        UpdateCount(dataManager.GetHealthPotionCount());
    }

    /// Subscribe to potionCountChanged event.
    void OnEnable()
    {
        PlayerItemHolder.potionCountChanged += UpdateCount;
    }

    /// Unsubscribe from potionCountChanged event.
    void OnDisable()
    {
        PlayerItemHolder.potionCountChanged -= UpdateCount;
    }

    /// Update text to display the new potion count.
    void UpdateCount(int newCount)
    {
        countText.text = newCount.ToString();
    }
}
