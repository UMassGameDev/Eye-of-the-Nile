/**************************************************
Holds the data of the ability items in the ability inventory in the skyhub.

Documentation updated 4/2/2024
**************************************************/
using UnityEngine;

public class AbilityInventoryItemData : MonoBehaviour
{
    public string abilityName;
    public int startingSlotKey;
    public int finalSlotKey { get; private set; }

    public void MoveIcon(Vector2 newPos) { gameObject.transform.position = newPos; }
}
