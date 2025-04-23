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
    /// Reference to the message that Ma'at says (set in inspector).
    [SerializeField] protected GameObject welcomeMessage;

    void Awake()
    {
        // Set reference to dataManager.
        dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
    }

    void Start()
    {
        welcomeMessage.SetActive(!dataManager.maatTalked);
    }

    /// Runs when Ma'at is talked to for the first time (and every time).
    public void MaatTalked()
    {
        // Hide the Ma'at message and keep it hidden.
        dataManager.maatTalked = true;
        welcomeMessage.SetActive(false);
    }
}