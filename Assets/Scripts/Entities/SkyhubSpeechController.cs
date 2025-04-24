using UnityEngine;

/** \brief
This script controls the welcome message(s) in the Skyhub so that they only appear on the player's first visit.

Documentation updated 4/22/2025
\author Alexander Art
*/
public class SkyhubSpeechController : MonoBehaviour
{
    /// Reference to the DataManager.
    DataManager dataManager;
    /// Reference to the message that Ma'at says. (These references can be set in the inspector.)
    [SerializeField] protected GameObject welcomeMessage;
    /// Reference to the message that the warp obelisk says.
    [SerializeField] protected GameObject returnMessage;
    /// Reference to the ability inventory tutorial.
    [SerializeField] protected GameObject abilityInventoryTutorial;

    void Awake()
    {
        // Set reference to dataManager.
        dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
    }

    void Start()
    {
        welcomeMessage.SetActive(!dataManager.maatTalked);
        returnMessage.SetActive(!dataManager.skyhubExited);
    }

    /// Runs when Ma'at is talked to for the first time (and every time).
    public void MaatTalked()
    {
        // Tutorial message pops up if this is the player's first time talking to Ma'at.
        if (dataManager.maatTalked == false)
        {
            abilityInventoryTutorial.SetActive(true);
        }
        else
        {
            abilityInventoryTutorial.SetActive(false);
        }

        // Hide Ma'at message and keep it hidden.
        dataManager.maatTalked = true;
        welcomeMessage.SetActive(false);
    }

    /// Runs when the player exits the Skyhub for the first time (and every time).
    public void SkyhubExited()
    {
        // Tell the DataManager that the player has exited the Skyhub.
        dataManager.skyhubExited = true;
    }
}