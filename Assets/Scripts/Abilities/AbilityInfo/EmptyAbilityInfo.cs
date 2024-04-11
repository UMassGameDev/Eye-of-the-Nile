/**************************************************
This is an empty ability that does nothing.
This allows for there to be empty slots in the active ability data.

Documentation updated 4/11/2024
**************************************************/
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EmptyAbilityInfo", menuName = "Abilities/Create New EmptyAbilityInfo")]
public class EmptyAbilityInfo : BaseAbilityInfo
{
    protected override void AbilityOffense(AbilityOwner abilityOwner) { Debug.Log("This ability does nothing"); }

    protected override void AbilityDefense(AbilityOwner abilityOwner) { Debug.Log("This ability does nothing"); }

    protected override void AbilityUtility(AbilityOwner abilityOwner) { Debug.Log("This ability does nothing"); }

    protected override void AbilityPassive(AbilityOwner abilityOwner) { Debug.Log("This ability does nothing"); }
}
