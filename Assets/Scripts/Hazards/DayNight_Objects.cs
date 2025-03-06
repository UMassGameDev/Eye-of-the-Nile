using UnityEngine;

/** \brief
This script goes on parents of objects that you want to be active during only day or only night.
This script has to go on the parent for two reasons:
1. Efficiency: one script can control many objects
2. If this script disables the object that it is attached to, it cannot reactive itself

Documentation updated 2/13/2025
\author Alexander Art
*/
public class DayNight_Objects : MonoBehaviour
{
    /// Reference to the DataManager.
    private DataManager dataManager;

    /// The time of day that the object will be active during.
    public TimeOfDay activeTime;

    /// The current state of this object's children. It will change according to the time of day. Children should start off active.
    private bool activeState = true;

    /// Set references.
    void Awake()
    {
        dataManager = DataManager.Instance != null ? DataManager.Instance : FindObjectOfType<DataManager>();
    }

    ///
    void Update()
    {
        // If the current active state of this object's children do not match what it should be, update them.
        if (activeState != (dataManager.GetTimeOfDay() == activeTime))
        {
            // Loop through each child of the object that this script is attached to and update their active state to what it should be.
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(dataManager.GetTimeOfDay() == activeTime);
            }

            // Invert activeState.
            activeState = !activeState;
        }
    }
}
