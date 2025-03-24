using UnityEngine;

/** \brief
This script has functions for opening and closing the door object that this script is attached to.
The door can be locked. The reason why a number is used for the doorLocked variable instead of a Boolean is, for example, if
two separate events need to contribute toward unlocking the door, the door lock value can be incremented by 1/2 of the total,
so the door won't get unlocked until both events happen.

Documentation updated 3/23/2025
\author Alexander Art
\todo (Maybe) Stop the player from being able to close the door on themself or other entities.
*/
public class DoorObject : MonoBehaviour
{
    /// Reference to the image of the closed door.
    [SerializeField] protected Sprite closedSprite;
    /// Reference to the image of the open door.
    [SerializeField] protected Sprite openSprite;
    /// Reference to door's collision that goes away when the door is open.
    [SerializeField] protected GameObject doorCollider;

    /// Whether the door should be open or closed when the scene is loaded.
    [SerializeField] protected bool openByDefault = false;
    /// If this number is greater than 0, then the door is locked.
    [SerializeField] protected float doorLocked = 0f;

    /// The current state of the door
    private bool doorOpen = false;

    void Start()
    {
        // Use openByDefault.
        if (openByDefault)
            ForceOpenDoor();
        else
            ForceCloseDoor();
    }

    /// Open the door, unless locked.
    public void OpenDoor()
    {
        if (doorLocked <= 0)
        {
            // Make the door appear open.
            GetComponent<SpriteRenderer>().sprite = openSprite;

            // Allow entities to pass through the door.
            doorCollider.SetActive(false);

            // Update the doorOpen variable.
            doorOpen = true;
        }
    }

    /// Open the door, even if locked.
    public void ForceOpenDoor()
    {
        // Make the door appear open.
        GetComponent<SpriteRenderer>().sprite = openSprite;

        // Allow entities to pass through the door.
        doorCollider.SetActive(false);

        // Update the doorOpen variable.
        doorOpen = true;
    }

    /// Close the door, unless locked.
    public void CloseDoor()
    {
        if (doorLocked <= 0)
        {
            // Make the door appear closed.
            GetComponent<SpriteRenderer>().sprite = closedSprite;

            // Stop entities from passing through the door.
            doorCollider.SetActive(true);

            // Update the doorOpen variable.
            doorOpen = false;
        }
    }

    /// Close the door, even if locked.
    public void ForceCloseDoor()
    {
        // Make the door appear closed.
        GetComponent<SpriteRenderer>().sprite = closedSprite;

        // Stop entities from passing through the door.
        doorCollider.SetActive(true);

        // Update the doorOpen variable.
        doorOpen = false;
    }

    /// Method for opening/closing the door: closes the door if open, opens the door if closed, unless locked.
    public void ToggleDoor()
    {
        if (doorOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    /// Alternative method for opening/closing the door: closes the door if open, opens the door if closed, even if locked.
    public void ForceToggleDoor()
    {
        if (doorOpen)
            ForceCloseDoor();
        else
            ForceOpenDoor();
    }

    /// Use this function to set the value of the lock. Anything greater than 0, and the door is locked.
    public void SetLock(float value) { doorLocked = value; }

    /// Use this function to increment the value of the lock.
    public void ChangeLock(float value) { doorLocked += value; }

    /// Returns true if the door is locked, otherwise false.
    public bool GetLocked() { return doorLocked > 0; }

    /// Returns the current open state of the door.
    public bool GetOpen() { return doorOpen; }
}
