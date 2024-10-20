using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/** \brief
Inheriting from KnockbackFeedback, this script will adjust the knockback resistance based on stat modifiers added to the player.

Documentation updated 10/11/2024
\author Stephen Nuttall
*/
public class PlayerKnockbackFeedback : KnockbackFeedback
{
    /// Subscribe to events.
    void OnEnable()
    {
        PlayerStat.modifierAdded += addKbResistance;
        PlayerStat.modifierRemoved += removeKbResistance;
    }

    /// Unsubscribe from events.
    void OnDisable()
    {
        PlayerStat.modifierAdded -= addKbResistance;
        PlayerStat.modifierRemoved -= removeKbResistance;
    }
    
    /// If the given stat modifier is a knockback resistance modifier, add the modifier's value to the overall knockback resistance.
    void addKbResistance(string modiferName, int modiferValue)
    {
        if (modiferName == "KnockbackResistance")
        {
            kbResistance += modiferValue;
        }
    }

    /// If the given stat modifier is a knockback resistance modifier, remove the modifier's value to the overall knockback resistance.
    void removeKbResistance(string modiferName, int modiferValue)
    {
        if (modiferName == "KnockbackResistance")
        {
            kbResistance -= modiferValue;
        }
    }
}