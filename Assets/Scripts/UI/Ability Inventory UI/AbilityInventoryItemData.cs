/**************************************************
Holds the data of the ability items in the ability inventory in the skyhub.

Documentation updated 4/2/2024
**************************************************/
using UnityEngine;
using UnityEngine.UI;

public class AbilityInventoryItemData : MonoBehaviour
{
    public string abilityName;
    public int abilityLevel;
    public Sprite sprite;
    public int startingSlotKey;
    public int finalSlotKey { get; private set; }

    public void MoveIcon(Vector2 newPos) { gameObject.transform.position = newPos; }
}
