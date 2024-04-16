/**************************************************
Holds the data of the drag and drop ability icons in the ability inventory in the skyhub.
This includes all data that the UI needs to display, as well as what slot the ability icon is in.

Documentation updated 4/16/2024
**************************************************/
using UnityEngine;
using System.Collections.Generic;

public class AbilityInventoryItemData : MonoBehaviour
{
    public string abilityName;
    public int abilityLevel;
    public Sprite sprite;
    public string quote;

    public List<Sprite> abilityIcons;
    public string[] abilityNames;
    public string[] abilityDescriptions;

    // used to return the icon to it's current slot if dragged to a place that isn't another open slot
    public int startingSlotKey;

    public void MoveIcon(Vector2 newPos) { gameObject.transform.position = newPos; }
}
