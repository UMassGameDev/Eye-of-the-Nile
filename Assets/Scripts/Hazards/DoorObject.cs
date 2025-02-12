using UnityEngine;

/** \brief
This script has functions for opening and closing the door object that this script is attached to.

Documentation updated 2/12/2025
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

    /// The current state of the door
    private bool doorOpen = false;

    void Start()
    {
        // Use openByDefault.
        if (openByDefault)
            OpenDoor();
        else
            CloseDoor();
    }

    /// Method for opening the door.
    public void OpenDoor()
    {
        // Make the door appear open.
        GetComponent<SpriteRenderer>().sprite = openSprite;

        // Allow entities to pass through the door.
        doorCollider.SetActive(false);

        // Update the doorOpen variable.
        doorOpen = true;
    }

    /// Method for closing the door.
    public void CloseDoor()
    {
        // Make the door appear closed.
        GetComponent<SpriteRenderer>().sprite = closedSprite;

        // Stop entities from passing through the door.
        doorCollider.SetActive(true);

        // Update the doorOpen variable.
        doorOpen = false;
    }

    /// Method for opening/closing the door: closes the door if open, opens the door if closed.
    public void ToggleDoor()
    {
        if (doorOpen)
            CloseDoor();
        else
            OpenDoor();
    }

    /// Returns the current state of the door.
    public bool GetOpen() { return doorOpen; }
}
